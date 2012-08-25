namespace MsdnWeb.Controllers

open Vosen.MAL.Content
open Microsoft.FSharp.Collections
open System.Web
open System.Web.Mvc
open Npgsql
open Dapper

type pair<'a,'b> = System.Collections.Generic.KeyValuePair<'a, 'b>
type RecommendationSet= { Ids : int array; Titles : string array; Rating: int; Editable: bool }
type Recommendations<'a> = { Masterpiece: 'a array; Great : 'a array; VeryGood : 'a array }
type RecommendationsWithKnown = { Ids : Recommendations<int>; Titles: Recommendations<string>; Known : pair<string, pair<int,int>> array }
type TitleQuery = { mutable id : int }
type TitleResult = { mutable Title : string }
type ResizeArray<'a> = System.Collections.Generic.List<'a>

[<HandleError>]
type RecommendController(recommender : Vosen.Juiz.FunkSVD.TitleRecommender, dbPath: string) =
    inherit Controller()

    member private this.UseConnection(func) =
        use npgConn = new NpgsqlConnection(dbPath)
        npgConn.Open()
        func(npgConn)

    static member public PickRecommended count (scoreList : pair<int,float> array) =
        let rate10 = ResizeArray()
        let rate9 = ResizeArray()
        let rate8 = ResizeArray()
        for kvp in scoreList do
            if kvp.Value >= 9.33 then
                rate10.Add(kvp)
            else if kvp.Value >= 8.67 then
                rate9.Add(kvp)
            else if kvp.Value >= 8.0 then
                rate8.Add(kvp)
        let comparer = { new System.Collections.Generic.IComparer<pair<int,float>> with member this.Compare(x,y) = sign (y.Value - x.Value)}
        rate10.Sort(comparer)
        let fin10 = Array.init (min count rate10.Count) (fun i -> rate10.[i].Key)
        if rate10.Count < count then
            rate9.Sort(comparer)
        let fin9 = Array.init (min (count - fin10.Length) rate9.Count) (fun i -> rate9.[i].Key)
        if (rate10.Count + rate9.Count) < count then
            rate8.Sort(comparer)
        let fin8 = Array.init (min (count - fin10.Length - fin9.Length) rate8.Count) (fun i -> rate8.[i].Key)
        { Masterpiece = fin10; Great = fin9; VeryGood = fin8}

    member private this.ResolveTitleName id =
        this.UseConnection(fun conn -> conn.Query<string>("SELECT COALESCE(\"EnglishName\", \"RomajiName\") FROM \"Anime\" WHERE \"Id\" = :id", { id = id }) |> Seq.head)

    [<AcceptVerbs(HttpVerbs.Get)>]
    member this.FromList() =
        this.RedirectToRoute("Home")

    [<AcceptVerbs(HttpVerbs.Post)>]
    member this.FromList(title : string[], id : int[], rating : int[]) =
        let correctRatings =
            match (title, id, rating) with
            | (null, _, _)
            | (_, null, _)
            | (_, _, null) -> Array.empty
            | _ ->
                // TODO: if lengths are different try to salvage something from this.
                Array.zip3 id rating title
                |> Array.choose(fun (id, rating, title) -> 
                    match (id, title) with
                    | _ when (id <= 0 || id >= recommender.TitlesCount) ->
                        if System.String.IsNullOrWhiteSpace(title) then
                            None
                        else
                            Vosen.Madarame.NameCompletion.Complete this.UseConnection 1 title 
                            |> function 
                               | s when Seq.isEmpty s -> None
                               | s ->
                                    let first = Seq.head s
                                    Some(pair(first.id, rating))
                    | _ -> Some(pair(id, rating)))

        let recIds = 
            correctRatings
            |> recommender.PredictUnknown
            |> RecommendController.PickRecommended 50
        let addTitleToId (ids : int array) =
            ids |> Array.map (this.ResolveTitleName)
        let recTitles = { Masterpiece = addTitleToId recIds.Masterpiece;  Great = addTitleToId recIds.Great; VeryGood = addTitleToId recIds.VeryGood }
        let known = correctRatings |> Array.map (fun kvp -> pair(this.ResolveTitleName kvp.Key, pair(kvp.Key, kvp.Value)))
        this.View({ Ids = recIds; Titles = recTitles; Known = known })

    [<AcceptVerbs(HttpVerbs.Get)>]
    member this.FromMAL() =
        this.RedirectToRoute("Home")

    static member LoginFromFullUrl (login : string) =
        let correct, uri = System.Uri.TryCreate(login, System.UriKind.Absolute)
        if not correct then
            None
        else
            let domainParts = uri.Host.Split('.')
            if domainParts.Length < 2 || domainParts.[domainParts.Length - 1] <> "net" || domainParts.[domainParts.Length - 2] <> "myanimelist" then
                None
            else
                let pathParts = uri.AbsolutePath.Split('/')
                if pathParts.Length <> 3 || pathParts.[0] <> "" || pathParts.[1] <> "profile" then
                    None
                else
                    Some(pathParts.[2])

    static member LoginFromInput (input : string) =
        match input with
        | _ when not (input.Contains("/")) -> Some(input)
        | _ ->
            match (RecommendController.LoginFromFullUrl input) with
            | Some(login) -> Some(login)
            | None -> RecommendController.LoginFromFullUrl ("http://" + input)

    [<AcceptVerbs(HttpVerbs.Post)>]
    member this.FromMAL(login : string) =
        let result =
            match (RecommendController.LoginFromInput login) with
            | None -> None
            | Some(correct) -> 
                let result = Vosen.MAL.Content.Extract.DownloadAllAnime(correct)
                match result.Response with
                | AnimelistResponse.Successs -> 
                    let rated = ResizeArray()
                    let seen = System.Collections.Generic.HashSet()
                    for rating in result.Ratings do
                        if rating.Rating > 0uy then
                            rated.Add(pair(rating.AnimeId, int(rating.Rating)))
                        else
                            seen.Add(rating.AnimeId) |> ignore
                    let recommendedIds = 
                        rated.ToArray()
                        |> recommender.PredictUnknown
                        |> Array.filter (fun pair -> not (seen.Contains(pair.Key)))
                        |> RecommendController.PickRecommended 50
                    let resolveTitles ids =
                        ids |> Array.map this.ResolveTitleName
                    let recommendedTitles = { Masterpiece = resolveTitles recommendedIds.Masterpiece ; Great = resolveTitles recommendedIds.Great; VeryGood = resolveTitles recommendedIds.VeryGood }
                    Some(Some(recommendedTitles), correct)
                | _ -> Some(None, correct)
        this.View(result)