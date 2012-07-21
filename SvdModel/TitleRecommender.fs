namespace Vosen.Madarame

open System.Collections.Generic
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.LinearAlgebra.Generic
open MathNet.Numerics.FSharp

// wrapper for a svd model supporting:
// * rating normalization/denormalization
// * title/documentId translation
type TitleRecommender(model : RawSvdModel, averages: float[], titleToDocumentMapping: int[], documentToTitleMapping : int[]) =

    static member Load(path : string) =
        let reader = Double.IO.MatlabMatrixReader(path)
        let matrices = reader.ReadMatrices()
        let terms = matrices.["Terms"].RowEnumerator() |> Seq.map snd |> Seq.toArray
        let documents = matrices.["Documents"]
        let averages = matrices.["Averages"].Row(0) |> Seq.toArray
        let titleMapping = matrices.["TitleMapping"].Row(0) |> Seq.map int |> Seq.toArray
        let documentMapping = matrices.["DocumentMapping"].Row(0) |> Seq.map int |> Seq.toArray
        TitleRecommender(RawSvdModel(terms, documents), averages, titleMapping, documentMapping)

    member this.PredictUnknown(ratings : (int * int) array) =
        ratings
        // filter out incorrect ids
        |> Array.filter (fun (id, rating) -> this.IsCorrectId id)
        // map ids to correct ones and normalize scores
        |> Array.map (fun (id, rating) -> 
            let docId = titleToDocumentMapping.[id]
            (docId, float(rating) - averages.[docId]))
        // send to recommender
        |> model.PredictUnknown
        // denormalize ids and scores back
        |> Array.mapi (fun docId avgRating -> (documentToTitleMapping.[docId], avgRating + averages.[docId]))

    member private this.IsCorrectId id =
        id >= 0 && id < titleToDocumentMapping.Length && titleToDocumentMapping.[id] >= 0

    member this.DocumentsCount 
        with get() = documentToTitleMapping.Length

    member this.TitlesCount 
        with get() = titleToDocumentMapping.Length