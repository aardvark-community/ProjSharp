open Proj
open Aardvark.Base
open System.IO
open Proj
#nowarn "9"


[<EntryPoint>]
let main argv =
    Aardvark.Init()
    

    let wkt = """GEOGCS["GCS_WGS_1984",DATUM["D_WGS_1984",SPHEROID["WGS_1984",6378137,298.257223563]],PRIMEM["Greenwich",0],UNIT["Degree",0.017453292519943295]]"""
    
    use src = Proj.Define "+proj=tmerc +lat_0=0 +lon_0=31 +k=1 +x_0=0 +y_0=-5000000 +ellps=bessel +pm=ferro  +towgs84=577.326,90.129,463.919,5.137,1.474,5.297,2.4232 +units=m +no_defs"
    use dst = Proj.Define wkt //"+proj=utm +zone=33 +datum=WGS84"
    let res =  Proj.Transform(src, dst, V3d(66040.561, 269995.685, 640.175))

    printfn "%A" res


    0