namespace Vosen.Madarame

open System.Collections.Generic
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Generic
open MathNet.Numerics.FSharp

type RawSvdModel(termMatrix : Vector<float>[], documentMatrix : Matrix<float>) =

    member inline private this.termsCount  = termMatrix.Length

    member this.PredictUnknown(ratings : seq<int * float>) =
        let scoresVector = ratings |> RawSvdModel.TranslateScoresToVector this.termsCount
        let documentVector = documentMatrix.Multiply(scoresVector :> Vector<float>)
        scoresVector 
        |> Seq.mapi (fun idx score ->
            match score with
            | 0.0 -> this.PredictSingle documentVector idx
            | _ -> 0.0)
        |> Seq.toArray

    member private this.PredictSingle(vector : Vector<float>) (termId : int) =
        vector.DotProduct(termMatrix.[termId])

    static member private TranslateScoresToVector (size : int) (scoreList : seq<int * float>) =
        let returnVector = Double.DenseVector(size)
        for (id, rating) in scoreList do
            returnVector.[id] <- rating
        returnVector