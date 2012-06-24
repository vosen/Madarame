namespace Vosen.Madarame.Numerics

open Microsoft.FSharp.NativeInterop
open Microsoft.FSharp.Math
open SafeNativeMethods
open System

type NativeSvd =
    val Vt : CMatrix<float>
    val Ut : CMatrix<float>
    val S : NativeArray<float>
    val private SvdPtr : nativeptr<svdrec>

    new(matrix: Matrix<float>, dims : int) =
        let pinnedColumns = PinnedArray.of_array matrix.InternalSparseColumnValues
        let pinnedRows = PinnedArray.of_array matrix.InternalSparseRowOffsets
        let pinnedValues = PinnedArray.of_array matrix.InternalSparseValues
        let mutable nativeMat = new smat(matrix.NumCols, matrix.NumRows, matrix.InternalSparseValues.Length, pinnedRows.Ptr, pinnedColumns.Ptr, pinnedValues.Ptr)
        pinnedColumns.Free(); pinnedRows.Free(); pinnedValues.Free()
        let svd_ptr = svdLAS2A(&nativeMat, dims)
        let svd_struct = svd_ptr |> NativePtr.read
        let vt_struct = svd_struct.Vt |> NativePtr.read
        let ut_struct = svd_struct.Ut |> NativePtr.read
        // weird assignment because Matrix<float> and svdlibc use different row/column formats
        { Ut = CMatrix(vt_struct.value |> NativePtr.read, vt_struct.rows, vt_struct.cols);
          Vt = CMatrix(ut_struct.value |> NativePtr.read, ut_struct.rows, ut_struct.cols);
          S = NativeArray(svd_struct.S, svd_struct.d);
          SvdPtr = svd_ptr }

    member this.Dispose() =
        svdFreeSVDRec(this.SvdPtr)

    interface IDisposable with
        member this.Dispose() =
            this.Dispose()