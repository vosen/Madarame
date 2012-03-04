// ugly workaround for vs2010
#I @"bin\Debug\"

#r "NameCompletionProvider.dll"
#r "ServiceStack.dll"
#r "ServiceStack.Interfaces.dll"
#r "System.Configuration.dll"

open Vosen.Madarame
open System
open ServiceStack.WebHost.Endpoints
open System.Configuration

type AppHost(connString) =
    inherit AppHostHttpListenerBase("Test service", typeof<NameCompletionService>.Assembly)
    override this.Configure container =
        container.Register<NameCompletionService>(fun c -> NameCompletionService(connString)) |> ignore
        base.Routes.Add<SearchQuery>("/test") |> ignore

let main() =
    let connString = ConfigurationManager.OpenExeConfiguration(@".\" + fsi.CommandLineArgs.[0]).ConnectionStrings.ConnectionStrings.[0].ConnectionString
    let appHost = new AppHost(connString)
    appHost.Init()
    appHost.Start("http://localhost:8080/")
    printfn "listening on http://localhost:8080/ ..."
    Console.ReadLine() |> ignore

main()