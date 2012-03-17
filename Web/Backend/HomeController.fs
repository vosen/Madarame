namespace MsdnWeb.Controllers

open System.Web
open System.Web.Mvc

type Rating = { Name : string; Value: string }

[<HandleError>]
type HomeController() =
    inherit Controller()

    static let Ratings =
        [|
            { Name = "10 - Masterpiece"; Value = "10"}
            { Name = "9 - Great"; Value = "9"}
            { Name = "8 - Very Good"; Value = "8"}
            { Name = "7 - Very Good"; Value = "7"}
            { Name = "6 - Fine"; Value = "6"}
            { Name = "5 - Average"; Value = "5"}
            { Name = "4 - Bad"; Value = "4"}
            { Name = "3 - Very Bad"; Value = "3"}
            { Name = "2 - Horrible"; Value = "2"}
            { Name = "1 - Unwatchable"; Value = "1"}
        |]

    member this.Index () =
        this.View(Ratings) :> ActionResult

    //member this.AddTitle() =
        
