#I @"bin\Debug\"

#r "zlib.net.dll"
#r "MathNet.Numerics.dll"
#r "NativeSvd.dll"
#r "FSharp.PowerPack.dll"

open Microsoft.FSharp.Math
open Vosen.Madarame.Numerics

let sparse = Matrix.initSparse 3 3 [| (0,0,1.0); (0,1,2.0); (0,2,3.0); (1,0,4.0); (1,1,5.0); (1,2,6.0); (2,0,7.0); (2,1,8.0); (2,2,9.0) |]
//let sparse = Matrix.initSparse 2 3 [| (0,0,1.0); (0,1,2.0); (0,2,3.0); (1,0,4.0); (1,1,5.0); (1,2,6.0) |]
//let sparse = Matrix.initSparse 2 2 [| (0,0,1.0); (0,1,2.0); (1,0,3.0); (1,1,4.0) |]
printfn "%A" sparse.StructuredDisplayAsArray
//let sparse = SparseMatrix(2,2, [| 1.0; 1.0; 0.0; 0.0 |])
//let sparse = SparseMatrix(1,2, [| 1.0; 0.0 |])
let svd = NativeSvd(sparse, 2)

(*
for i = 0 to svd.Vt.Length do
    for j = 0  to svd.Vt.[i].Length do
        printfn "(%d, %d): %f" i j (svd.Vt.[i].Item(j))
    printfn ""
*)
for i = 0 to svd.Vt.NumRows-1 do
    for j = 0  to svd.Vt.NumCols-1 do
        printfn "(%d, %d): %f" i j (svd.Vt.Item(i,j))
    printfn ""
printfn ""
for i = 0 to svd.Ut.NumRows-1 do
    for j = 0  to svd.Ut.NumCols-1 do
        printfn "(%d, %d): %f" i j (svd.Ut.Item(i,j))
    printfn ""

for i = 0 to svd.S.Length do
    printfn "%d: %f" i (svd.S.Item(i))