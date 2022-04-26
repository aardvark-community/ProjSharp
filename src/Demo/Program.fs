open Proj
open Aardvark.Base
open System.IO
open Proj
#nowarn "9"

[<EntryPoint>]
let main argv =
    Aardvark.Init()

    use p = new Proj()

    let wkt = """GEOGCS["GCS_WGS_1984",DATUM["D_WGS_1984",SPHEROID["WGS_1984",6378137,298.257223563]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]]"""
    use t = 
        p.CreateTransformation(
            wkt, //"+proj=longlat +datum=WGS84 +no_defs", 
            "+proj=utm +zone=33 +datum=WGS84"
        )

    let res = t.Transform [|V3d(16.0, 48.0, 0.0)|]
    printfn "%A" res

    0