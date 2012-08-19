open System

type pair<'a,'b> = System.Collections.Generic.KeyValuePair<'a, 'b>

let method1 (rat : float array) = 
    let keys = Array.init rat.Length id
    System.Array.Sort(rat, keys, { new System.Collections.Generic.IComparer<float> with member this.Compare(x,y) = System.Math.Sign(y-x) })
    let i = ref (-1)
    Array.zip rat keys |> Seq.takeWhile (fun (score, idx) -> i := !i + 1; !i < 50 && score > 0.7) |> Seq.toArray |> Array.unzip |> snd

// buggy, doesn't work
let method2 (rat : float array) = 
    let set = Collections.Generic.SortedDictionary({ new System.Collections.Generic.IComparer<float> with member this.Compare(x,y) = System.Math.Sign(y-x) })
    for i = 0 to (rat.Length - 1) do
        set.Add(rat.[i], i)
    let i = ref (-1)
    set |> Seq.takeWhile (fun pair -> i := !i + 1; !i < 50 && pair.Key > 0.7) |> Seq.toArray |> Array.map (fun pair -> pair.Value)

let method3 (rat : float array) = 
    let keys = Array.init rat.Length id
    System.Array.Sort(rat,keys)
    let i = ref (-1)
    Array.zip rat keys |> Seq.takeWhile (fun (score, idx) -> i := !i + 1; !i < 50 && score > 0.7) |> Seq.map snd

let method4 (rat : float array) = 
    let keys = Array.init rat.Length id
    System.Array.Sort(rat,keys, { new System.Collections.Generic.IComparer<float> with member this.Compare(x,y) = System.Math.Sign(y-x) })
    let fin = System.Array.FindIndex(rat, (fun x -> x <= 0.7)) |> max 0 |> min 50
    Array.sub keys 0 fin

let method5 (rat : float array) =
    let rate10 = System.Collections.Generic.List()
    for i=0 to (rat.Length - 1) do
        let v = rat.[i]
        if v > 0.4 then
            rate10.Add(pair(i,v))
    let r10 = rate10 |> Seq.toArray 
    r10 |> Array.sortInPlaceBy (fun p -> p.Value)
    r10 |> Seq.take 50 |> Seq.toArray |> Array.map (fun p-> p.Key)

let PickRecommended (scoreList : float array) =
        let rec pick (scores : (int * float) array) idx rate10 rate9 rate8 =
            match idx with
            | _ when idx >= scores.Length -> (rate10, rate9, rate8)
            | _ -> 
                match scores.[idx] with
                | (id, over9) when (over9 >= 0.9) -> pick scores (idx+1) (id :: rate10) rate9 rate8
                | (id, over8)  when (over8 >= 0.8) -> pick scores (idx+1) rate10 (id :: rate9) rate8
                | (id, over7)  when (over7 >= 0.7) -> pick scores (idx+1) rate10 rate9 (id :: rate8)
                | _ -> pick scores (idx+1) rate10 rate9 rate8
        let zip = Array.zip (Array.init scoreList.Length id) scoreList
        pick zip 0 [] [] []

let rand = Random()
let ratings = Array.init 5000000 (fun _ -> rand.NextDouble())
#time
//let res1 = method1(ratings)
//method3(ratings)
//let res4 = method4(ratings)
let res5 = method5(ratings)
//printfn "%d" res5.Length
//let r10,r9,r8 = PickRecommended(ratings)
//printfn "%d, %d, %d" r10.Length r9.Length r8.Length