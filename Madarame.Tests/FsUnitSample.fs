namespace Vosen.Madarame.Tests

open NUnit.Framework
open FsUnit

[<TestFixture>] 
type WebBackend() =

    [<Test>]
    member this.``predicted results are filtered correctly``() =
        let scores = [| (0, 1.0); (1, 7.56); (2, 10.0); (20, 9.01); (11, 8.99); (3,7.1111); (9, 6.54); (13, 7.11); |]
        let picked = MsdnWeb.Controllers.RecommendController.PickRecommended(scores)
        picked.VeryGood |> should haveLength 3
        picked.VeryGood |> should contain 1
        picked.VeryGood |> should contain 3
        picked.VeryGood |> should contain 13
        picked.Great |> should haveLength 1
        picked.Great |> should contain 11
        picked.Masterpiece |> should haveLength 2
        picked.Masterpiece |> should contain 2
        picked.Masterpiece |> should contain 20
