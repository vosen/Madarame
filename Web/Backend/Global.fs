namespace MsdnWeb

open MsdnWeb.RouteCollectionExtensions
open System
open System.Web
open System.Web.Mvc
open System.Web.Routing

type Global() =
    inherit System.Web.HttpApplication() 

    static member RegisterRoutes(routes:RouteCollection) =
        let inline (=>) a b = a, box b

        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")
        routes.MapRoute("",
                        "recommend/list",
                        dict [ "controller" => "Recommend"; "action" => "FromList" ]) |> ignore
        routes.MapRoute("",
                        "recommend/mal/{login}",
                        dict [ "controller" => "Recommend"; "action" => "FromMAL" ]) |> ignore
        routes.MapRoute("Default",
                        "{controller}/{action}/{id}",
                        dict [ "controller" => "Home"; "action" => "Index"; "id" => UrlParameter.Optional ]) |> ignore

    member this.Start() =
        AreaRegistration.RegisterAllAreas()
        Global.RegisterRoutes(RouteTable.Routes)