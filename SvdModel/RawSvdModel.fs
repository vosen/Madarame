namespace Vosen.Madarame

open System.Collections.Generic
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Generic
open MathNet.Numerics.FSharp

type RawSvdModel(termMatrix : Vector<float32>[], documentMatrix : Matrix<float32>) =

    member inline private this.termsCount  = termMatrix.Length

    member this.PredictUnknown(ratings : seq<int * float32>) =
        let scoresVector = ratings |> RawSvdModel.TranslateScoresToVector this.termsCount
        let documentVector = documentMatrix.Multiply(scoresVector :> Vector<float32>)
        scoresVector 
        |> Seq.mapi (fun idx score ->
            match score with
            | 0.0f -> this.PredictSingle documentVector idx
            | _ -> 0.0f)
        |> Seq.toArray

    member private this.PredictSingle(vector : Vector<float32>) (termId : int) =
        vector.DotProduct(termMatrix.[termId])

    static member private TranslateScoresToVector (size : int) (scoreList : seq<int * float32>) =
        let returnVector = Single.DenseVector(size)
        for (id, rating) in scoreList do
            returnVector.[id] <- float32(rating)
        returnVector