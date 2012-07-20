namespace MsdnWeb

module RouteCollectionExtensions =
    open System
    open System.Web.Routing
    open System.Collections.Generic

    type RouteCollection with

        member routes.MapRoute(url : string, defaults : IDictionary<string, obj>) =
            let route = System.Web.Mvc.RouteCollectionExtensions.MapRoute(routes, null, url)
            route.Defaults <- RouteValueDictionary(defaults)
            route

        member routes.MapRoute(url : string, defaults : IDictionary<string, obj>, constraints : IDictionary<string, obj>) =
            let route = System.Web.Mvc.RouteCollectionExtensions.MapRoute(routes, null, url)
            route.Defaults <- RouteValueDictionary(defaults)
            route.Constraints <- RouteValueDictionary(constraints)
            route