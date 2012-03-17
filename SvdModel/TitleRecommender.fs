namespace Vosen.Madarame

open System.Collections.Generic
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Generic
open MathNet.Numerics.FSharp

// wrapper for a svd model supporting:
// * rating normalization/denormalization
// * title/documentId translation
type TitleRecommender(model : RawSvdModel, averages: float32[], titleToDocumentMapping: int[], documentToTitleMapping : int[]) =

    static member Load(path : string) =
        let reader = Single.IO.MatlabMatrixReader(path)
        let matrices = reader.ReadMatrices()
        let terms = matrices.["Terms"].RowEnumerator() |> Seq.map snd |> Seq.toArray
        let documents = matrices.["Documents"]
        let averages = matrices.["Averages"].Column(0) |> Seq.toArray
        let titleMapping = matrices.["TitleMapping"].Column(0) |> Seq.map int |> Seq.toArray
        let documentMapping = matrices.["DocumentMapping"].Column(0) |> Seq.map int |> Seq.toArray
        TitleRecommender(RawSvdModel(terms, documents), averages, titleMapping, documentMapping)

    member this.PredictUnknown(ratings : seq<int * int>) =
        ratings
        // filter out incorrect ids
        |> Seq.filter (fun (id, rating) -> this.IsCorrectId id)
        // map ids to correct ones and normalize scores
        |> Seq.map (fun (id, rating) -> (titleToDocumentMapping.[id], averages.[id] - float32(rating)))
        // send to recommender
        |> model.PredictUnknown
        // denormalize ids and scores back
        |> Array.mapi (fun docId avgRating -> (documentToTitleMapping.[docId], avgRating + averages.[docId]))

    member private this.IsCorrectId id =
        id >= 0 && id < titleToDocumentMapping.Length && titleToDocumentMapping.[id] >= 0