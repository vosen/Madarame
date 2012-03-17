namespace Vosen.Madarame

open NUnit.Framework
open FsUnit
open MathNet.Numerics.LinearAlgebra

[<TestFixture>] 
type Svd() as this =
    [<DefaultValue>] val mutable model : RawSvdModel
    [<DefaultValue>] val mutable averages : float32 array
    // adapted from page 8 from programming collab intelligence
    do
        let rawArray = [| [|5.0f; 6.0f; 5.0f; 0.0f; 6.0f; 6.0f; 0.0f|]; [|7.0f; 7.0f; 6.0f; 7.0f; 8.0f; 8.0f; 9.0f|]; [|6.0f; 3.0f; 0.0f; 6.0f; 4.0f; 0.0f; 0.0f|]; [|7.0f; 10.0f; 7.0f; 8.0f; 6.0f; 10.0f; 8.0f|]; [|5.0f; 7.0f; 0.0f; 5.0f; 2.0f; 7.0f; 2.0f|]; [|6.0f; 6.0f; 8.0f; 9.0f; 6.0f; 6.0f; 0.0f|] |]
        let averages = rawArray |> Seq.map (Seq.filter (function | 0.0f -> false | _ -> true) >> Seq.average) |> Seq.toArray
        let scoreMatrix = Single.DenseMatrix(Array2D.init 6 7 (fun x y ->
            match rawArray.[x].[y] with
            | 0.0f -> rawArray.[x].[y]
            | _ -> rawArray.[x].[y] - averages.[x]))
        let svd = Single.Factorization.DenseSvd(scoreMatrix, true)
        let U = svd.U()
        let E = new Single.DiagonalMatrix(svd.Rank, svd.Rank, svd.S().ToArray())
        let Esqrt = new Single.DenseMatrix(svd.Rank, svd.Rank)
        Esqrt.SetDiagonal(svd.S() |> Seq.map sqrt |> Seq.toArray)
        let terms = U.Multiply(Esqrt)
        let documents = Esqrt.Multiply(E.Inverse()).Multiply(U)
        this.model <- RawSvdModel(terms.RowEnumerator() |> Seq.map (fun (i, r) -> r) |> Seq.toArray, documents)
        this.averages <- averages

    member private this.Normalize arr =
        Array.map (fun (id, rat) -> (id, this.averages.[id] - float32(rat))) arr
    [<Test>]
    member this.``prediction should predict only unknown``()=
        let prediction = [| (0,5); (1,7); (3,7); (4,5); (5,6)|] |> this.Normalize |> this.model.PredictUnknown
        prediction |> Array.filter (function | 0.0f -> false | _ -> true) |> should haveLength 1