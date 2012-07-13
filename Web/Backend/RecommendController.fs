namespace MsdnWeb.Controllers

open System.Web
open System.Web.Mvc
open System.Collections.Generic

[<HandleError>]
type RecommendController() =
    inherit Controller()

    [<AcceptVerbs(HttpVerbs.Post)>]
    member this.FromList(title : string[], id : int[], rating : int[]) =
        match (id, rating) with
        | (null, _)
        | (_, null) -> this.View()
        | _ ->
            Seq.zip id rating
            |> Seq.filter(fun (id, rating) -> 
                match id with
                | _ when id <= 0 -> false
                | _ -> true)
            |> ignore
            this.View()

    member this.FromMAL(login : string) =
        this.View()
 
