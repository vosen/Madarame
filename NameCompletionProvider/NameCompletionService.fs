module Vosen.Madarame.NameCompletion

open System
open ServiceStack.ServiceHost
open System.Configuration
open Npgsql
open System.Data
open Dapper

type SearchQuery = { mutable term : string }
type AnimeTitle = { mutable value : string; mutable id : int }
type DbQuery = { term: string; limit: int }

let Complete connFunc resultLimit (term : string) : AnimeTitle seq =
        match term with
        | null -> Seq.empty
        | _ ->
            match term.Length with
            | _ when (term.Length < 3) -> Seq.empty
            | _ -> connFunc(fun (conn : NpgsqlConnection) ->
                        conn.Query<AnimeTitle>("SELECT title as value, id as id FROM find_title(:term, :limit);", { term = term; limit = resultLimit } ))

type Service(connString) =
    static member Complete ((term : string), connFunc, (?termLimit: int), (?resultLimit: int)) : AnimeTitle seq =
        let termLimit = defaultArg termLimit 3
        let resultLimit = defaultArg resultLimit 20
        match term with
        | null -> Seq.empty
        | _ ->
            match term.Length with
            | _ when (term.Length < termLimit) -> Seq.empty
            | _ -> connFunc(fun (conn : NpgsqlConnection) ->
                        conn.Query<AnimeTitle>("SELECT title as value, id as id FROM find_title(:term, :limit);", { term = term; limit = resultLimit } ))

    member this.UseConnection(func) =
        use npgConn = new NpgsqlConnection(connString.ToString())
        npgConn.Open()
        func(npgConn)


    interface IService<SearchQuery> with
        member this.Execute (query) = box (Complete this.UseConnection 10 query.term)