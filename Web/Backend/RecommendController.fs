namespace MsdnWeb.Controllers

open System.Web
open System.Web.Mvc
open Npgsql
open Dapper

type Recommendations = { Masterpiece: string list; Great : string list; VeryGood : string list }
type RecommendationsIds = { Masterpiece: int list; Great : int list; VeryGood : int list }
type TitleQuery = { mutable id : int }
type TitleResult = { mutable Title : string }

[<HandleError>]
type RecommendController(recommender : Vosen.Juiz.FunkSVD.TitleRecommender, dbPath: string) =
    inherit Controller()

    member private this.UseConnection(func) =
        use npgConn = new NpgsqlConnection(dbPath)
        npgConn.Open()
        func(npgConn)

    static member public PickRecommended scoreList =
        let rec pick (scores : (int * float) array) idx rate10 rate9 rate8 =
            match idx with
            | _ when idx >= scores.Length -> { Masterpiece = rate10; Great = rate9; VeryGood = rate8 }
            | _ -> 
                match scores.[idx] with
                | (id, over9) when (over9 >= 9.0) -> pick scores (idx+1) (id :: rate10) rate9 rate8
                | (id, over8)  when (over8 >= 8.0) -> pick scores (idx+1) rate10 (id :: rate9) rate8
                | (id, over7)  when (over7 >= 7.0) -> pick scores (idx+1) rate10 rate9 (id :: rate8)
                | _ -> pick scores (idx+1) rate10 rate9 rate8
        pick scoreList 0 [] [] []

    member private this.ResolveRecommendedTitles (recids : RecommendationsIds) : Recommendations  =
        let mapToNames (ids : int list) =
            ids
            |> List.map (fun id ->
                (this.UseConnection(fun conn -> conn.Query<TitleResult>("SELECT \"RomajiName\" AS \"Title\" FROM \"Anime\" WHERE \"Id\" = :id", { id = id }))
                |> Seq.head).Title)
        { Masterpiece = (mapToNames recids.Masterpiece); Great = (mapToNames recids.Great); VeryGood = (mapToNames recids.VeryGood) }

    [<AcceptVerbs(HttpVerbs.Post)>]
    member this.FromList(title : string[], id : int[], rating : int[]) =
        let correctRatings =
            match (title, id, rating) with
            | (null, _, _)
            | (_, null, _)
            | (_, _, null) -> null
            | _ -> 
                Array.zip3 id rating title
                |> Array.choose(fun (id, rating, title) -> 
                    match id with
                    | _ when (id <= 0 || id >= recommender.TitlesCount) ->
                        Vosen.Madarame.NameCompletion.Complete this.UseConnection 1 title 
                        |> function 
                           | s when Seq.isEmpty s -> None
                           | s ->
                                let first = Seq.head s
                                Some(first.id, rating)
                    | _ -> Some(id, rating))
        
        match correctRatings with
        | empty when empty = null -> this.View()
        | _ ->
            let predictions = 
                correctRatings
                |> recommender.PredictUnknown
                |> RecommendController.PickRecommended
                |> this.ResolveRecommendedTitles
            this.View(predictions)

    member this.FromMAL(login : string) =
        this.View(box null)
 
