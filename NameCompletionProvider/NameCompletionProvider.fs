namespace Vosen.Madarame

open System
open ServiceStack.ServiceHost

type SearchQuery = { term : string }
type Anime = { Name : string; Id : int }

type NameCompletionService() = 
    interface IService<SearchQuery> with
        member this.Execute (query) = 
            [| { Name = query.term; Id = 100 } |] :> obj
