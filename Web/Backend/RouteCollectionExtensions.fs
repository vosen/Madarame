namespace MsdnWeb

module RouteCollectionExtensions =
    open System
    open System.Web.Routing
    open System.Collections.Generic

    type RouteCollection with
        member routes.MapRoute(name : string, url : string, defaults : IDictionary<string, obj>) =
            let route = System.Web.Mvc.RouteCollectionExtensions.MapRoute(routes, name, url)
            route.Defaults <- RouteValueDictionary(defaults)
            route