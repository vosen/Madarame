module internal Vosen.Madarame.Numerics.SafeNativeMethods

open System.Runtime.InteropServices 
open Microsoft.FSharp.NativeInterop

[<StructLayoutAttribute(LayoutKind.Sequential)>]
type smat =
    struct
        val mutable rows : int
        val mutable cols : int
        val mutable vals : int
        val mutable pointr : nativeptr<int>
        val mutable rowind : nativeptr<int>
        val mutable value : nativeptr<float>
        new(rows, cols, vals, pointr, rowind, value) =
            { rows = rows; cols = cols; vals = vals; pointr = pointr; rowind = rowind; value = value }
    end

[<StructLayoutAttribute(LayoutKind.Sequential)>]
type dmat =
    struct
        val mutable rows : int
        val mutable cols : int
        // double**
        val mutable value : nativeptr<nativeptr<double>>
    end

[<StructLayoutAttribute(LayoutKind.Sequential)>]
type svdrec =
    struct
        val mutable d : int
        // dmat*
        val mutable Ut : nativeptr<dmat>
        // double*
        val mutable S : nativeptr<float>
        // dmat*
        val mutable Vt : nativeptr<dmat>
    end

type svdrec_ptr = nativeptr<svdrec> 

[<DllImport("svdlibc", CallingConvention = CallingConvention.Cdecl)>]
extern svdrec_ptr svdLAS2A(smat& A, int dimensions)

[<DllImport("svdlibc", CallingConvention = CallingConvention.Cdecl)>]
extern void svdFreeSVDRec(svdrec_ptr svd)