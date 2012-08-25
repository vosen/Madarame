namespace Vosen.Madarame.Tests

open NUnit.Framework
open FsUnit

type pair<'a,'b> = System.Collections.Generic.KeyValuePair<'a,'b>

[<TestFixture>] 
type WebBackend() =

    [<Test>]
    member this.``predicted results are filtered correctly when count is large``() =
        let scores = [| pair(0, 1.0); pair(1, 7.56); pair(2, 10.0); pair(20, 9.01); pair(11, 8.99); pair(3,7.1111); pair(9, 6.54); pair(13, 7.11); |]
        let picked = MsdnWeb.Controllers.RecommendController.PickRecommended 50 scores
        picked.VeryGood |> should haveLength 0
        picked.Great |> should haveLength 2
        picked.Great |> should contain 11
        picked.Great |> should contain 20
        picked.Masterpiece |> should haveLength 1
        picked.Masterpiece |> should contain 2

    [<Test>]
    member this.``predicted results are filtered correctly when count is small``() =
        let scores = [| pair(0, 1.0); pair(1, 7.56); pair(2, 10.0); pair(20, 9.01); pair(11, 8.99); pair(3,7.1111); pair(9, 6.54); pair(13, 7.11); |]
        let picked = MsdnWeb.Controllers.RecommendController.PickRecommended 2 scores
        picked.VeryGood |> should haveLength 0
        picked.Great |> should haveLength 1
        picked.Great |> should contain 20
        picked.Masterpiece |> should haveLength 1
        picked.Masterpiece |> should contain 2

    [<Test>]
    member this.``MAL recommendation correctly parses full URL``() =
        let address1 = "http://myanimelist.net/profile/askanier"
        let address2 = "http://www.myanimelist.net/profile/askanier"
        MsdnWeb.Controllers.RecommendController.LoginFromInput address1 |> should equal (Some("askanier"))
        MsdnWeb.Controllers.RecommendController.LoginFromInput address2 |> should equal (Some("askanier"))

    [<Test>]
    member this.``MAL recommendation correctly parses short URL``() =
        let address1 = "myanimelist.net/profile/askanier"
        let address2 = "www.myanimelist.net/profile/askanier"
        MsdnWeb.Controllers.RecommendController.LoginFromInput address1 |> should equal (Some("askanier"))
        MsdnWeb.Controllers.RecommendController.LoginFromInput address2 |> should equal (Some("askanier"))

    [<Test>]
    member this.``MAL recommendation correctly parses just login``() =
        let address1 = "askanier"
        MsdnWeb.Controllers.RecommendController.LoginFromInput address1 |> should equal (Some("askanier"))
