namespace MsdnWeb.Controllers

open System.Web
open System.Web.Mvc
open System.Collections.Generic

[<HandleError>]
type RecommendController() =
    inherit Controller()

    [<AcceptVerbs(HttpVerbs.Post)>]
    member this.FromList(titles : string[], ids : int[], ratings : int[]) =
        match (ids, ratings) with
        | (null, _)
        | (_, null) -> this.View()
        | _ ->
            Seq.zip ids ratings
            |> Seq.filter(fun (id, rating) -> 
                match id with
                | _ when id <= 0 -> false
                | _ -> true)
            |> ignore
            this.View()

    member this.FromMAL(login : string) =
        this.View()
 
