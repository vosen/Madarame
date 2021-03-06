﻿namespace MsdnWeb

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
        routes.MapRoute(null,
                        "recommend/list",
                        dict [ "controller" => "Recommend"; "action" => "FromList" ]) |> ignore
        routes.MapRoute(null,
                        "recommend/mal",
                        dict [ "controller" => "Recommend"; "action" => "FromMAL" ]) |> ignore
        routes.MapRoute("Home",
                        "",
                        dict [ "controller" => "Home"; "action" => "Index"; ]) |> ignore

    member this.Start() =
        AreaRegistration.RegisterAllAreas()
        Global.RegisterRoutes(RouteTable.Routes)