open Proj
open Aardvark.Base
open System.IO
open Proj
#nowarn "9"

[<EntryPoint>]
let main argv =
    Aardvark.Init()

    use p = new Proj()


    use t = 
        p.CreateTransformation(
            "+proj=longlat +datum=WGS84 +no_defs", 
            "+proj=utm +zone=33 +datum=WGS84"
        )

    let res = t.Transform [|V3d(16.0, 48.0, 0.0)|]
    printfn "%A" res

    0