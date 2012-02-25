// ugly workaround for vs2010
#I @"bin\Debug\"

#r "NameCompletionProvider.dll"
#r "ServiceStack.dll"
#r "ServiceStack.Interfaces.dll"

open Vosen.Madarame
open System
open ServiceStack.WebHost.Endpoints

type AppHost =
    inherit AppHostHttpListenerBase
    new() = { inherit AppHostHttpListenerBase("NameCompletionService", typeof<NameCompletionService>.Assembly) }
    override this.Configure container =
        base.Routes
            .Add<SearchQuery>("/test") |> ignore 

let main() =
    let appHost = new AppHost()
    appHost.Init()
    appHost.Start("http://localhost:8080/")
    printfn "listening on http://localhost:8080/ ..."
    Console.ReadLine() |> ignore

main()