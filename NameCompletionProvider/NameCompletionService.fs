namespace Vosen.Madarame

open System
open ServiceStack.ServiceHost
open System.Configuration
open Npgsql
open System.Data
open Dapper

type SearchQuery = { mutable term : string }
type AnimeResponse = { mutable label : string; mutable value : int }
type DbQuery = { term: string; limit: int }

type NameCompletionService(connString) =
    member this.UseConnection(func) =
        use npgConn = new NpgsqlConnection(connString.ToString())
        npgConn.Open()
        func(npgConn)

    interface IService<SearchQuery> with
        member this.Execute (query) = 
            match query.term with
            | null -> Array.empty :> obj
            | _ ->
                match query.term.Length with
                | _ when (query.term.Length < 3) -> Array.empty :> obj
                | _ -> this.UseConnection(fun conn ->
                    conn.Query<AnimeResponse>("SELECT title as label, id as value FROM find_title(:term, :limit);", { term = query.term; limit = 20 } )) :> obj