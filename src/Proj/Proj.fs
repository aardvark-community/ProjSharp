namespace Proj

open System
open System.Threading
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop
open Aardvark.Base

#nowarn "9"


module internal UnpackData =
    type Marker = class end
    open System
    open System.IO
    open System.IO.Compression

    let run() =
        let ass = typeof<Marker>.Assembly
        let version =
            ass.GetCustomAttributes(true) 
            |> Seq.tryPick (function 
                | :? System.Reflection.AssemblyInformationalVersionAttribute as o -> Some o.InformationalVersion 
                | _ -> None
            )
        
        let name = 
            match version with
            | Some v -> Path.Combine("ProjSharp", v)
            | None -> "ProjSharp"

        let dataPath = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData), name)

        if not (Directory.Exists dataPath) then 
            Directory.CreateDirectory dataPath |> ignore

        match ass.GetManifestResourceNames() |> Array.tryFind (fun r -> r.Trim() = "native.zip") with
        | Some nativeName ->
            use archive = new ZipArchive(ass.GetManifestResourceStream(nativeName))
            for e in archive.Entries do
                let parts = e.FullName.Replace("\\", "/").Split('/', StringSplitOptions.RemoveEmptyEntries)
                if parts.Length > 1 && parts.[0] = "data" then

                    let outFile = Path.Combine(dataPath, Path.Combine(parts))
                    if not (File.Exists outFile) then
                        let dir = Path.GetDirectoryName outFile
                        if not (Directory.Exists dir) then Directory.CreateDirectory dir |> ignore
                        use d = File.OpenWrite outFile
                        use s = e.Open()
                        s.CopyTo d
            Some (Path.Combine(dataPath, "data"))
        | None ->
            None


module ProjRaw =

    type ContextHandle =
        struct
            val mutable public Handle : nativeint

            member x.IsNull = x.Handle = 0n
            member x.IsValid = x.Handle <> 0n
            static member Null = ContextHandle(Handle = 0n)
        end

    type CoordinateSystemHandle =
        struct
            val mutable public Handle : nativeint
            member x.IsNull = x.Handle = 0n
            member x.IsValid = x.Handle <> 0n
            static member Null = CoordinateSystemHandle(Handle = 0n)
        end

    type TransformationHandle =
        struct
            val mutable public Handle : nativeint
            member x.IsNull = x.Handle = 0n
            member x.IsValid = x.Handle <> 0n
            static member Null = TransformationHandle(Handle = 0n)
        end

    [<DllImport("ProjNative")>]
    extern void pInit([<MarshalAs(UnmanagedType.LPUTF8Str)>] string searchPath);

    [<DllImport("ProjNative")>]
    extern ContextHandle pCreateContext();
    
    [<DllImport("ProjNative")>]
    extern void pDestroyContext(ContextHandle context);

    // [<DllImport("ProjNative")>]
    // extern TransformationHandle pCreateTransformation(ContextHandle context, [<MarshalAs(UnmanagedType.LPStr)>] string src, [<MarshalAs(UnmanagedType.LPStr)>] string dst)


    [<DllImport("ProjNative")>]
    extern CoordinateSystemHandle pCreateCoordinateSystem(ContextHandle context, [<MarshalAs(UnmanagedType.LPUTF8Str)>] string definition)

    [<DllImport("ProjNative")>]
    extern void pDestroyCoordinateSystem(CoordinateSystemHandle system)

    [<DllImport("ProjNative")>]
    extern TransformationHandle pCreateCoordinateTransformation(ContextHandle context, CoordinateSystemHandle src, CoordinateSystemHandle dst, nativeint options)



    [<DllImport("ProjNative")>]
    extern void pDestroyTransformation(TransformationHandle trans)

    [<DllImport("ProjNative")>]
    extern bool pTransformArray(TransformationHandle trans, int count, V3d* inPoints, V3d* outPoints)

    [<DllImport("ProjNative", EntryPoint = "pTransformArray")>]
    extern bool pTransformByRef(TransformationHandle trans, int count, V3d& inPoint, V3d& outPoint)


    [<DllImport("ProjNative")>]
    extern bool pTransformArray2d(TransformationHandle trans, int count, V2d* inPoints, V2d* outPoints)

    [<DllImport("ProjNative", EntryPoint = "pTransformArray2d")>]
    extern bool pTransformByRef2d(TransformationHandle trans, int count, V2d& inPoint, V2d& outPoint)


type Transformation(parent : Context, handle : ProjRaw.TransformationHandle, src : CoordinateSystem, dst : CoordinateSystem) =
    let mutable handle = handle
 
    override x.ToString() =
        sprintf "Transformation[%A -> %A]" src dst 

    member x.Transform(point : V3d) =
        let mutable res = V3d.Zero
        let mutable point = point
        if ProjRaw.pTransformByRef(handle, 1, &point, &res) then
            res
        else
            failwith "could not transform"

    member x.Transform(point : V2d) =
        let mutable res = V3d.Zero
        let mutable point = V3d(point, 0.0)
        if ProjRaw.pTransformByRef(handle, 1, &point, &res) then
            res.XY
        else
            failwith "could not transform"

    member x.Transform(points : V3d[]) = 
        let res = Array.zeroCreate points.Length
        use src = fixed points
        use dst = fixed res
        if ProjRaw.pTransformArray(handle, points.Length, src, dst) then
            res
        else
            failwith "could not transform"
        
    member x.Transform(points : V2d[]) = 
        let res = Array.zeroCreate points.Length
        use src = fixed points
        use dst = fixed res
        if ProjRaw.pTransformArray2d(handle, points.Length, src, dst) then
            res
        else
            failwith "could not transform"

    member private x.Dispose(disposing : bool) =
        if disposing then GC.SuppressFinalize x
        if handle.IsValid then
            let h = handle
            parent.Start (fun () -> ProjRaw.pDestroyTransformation h; -1)
            handle <- ProjRaw.TransformationHandle.Null

    member x.Dispose() = x.Dispose true
    override x.Finalize() = x.Dispose false

    interface IDisposable with
        member x.Dispose() = x.Dispose()

and CoordinateSystem(parent : Context, handle : ProjRaw.CoordinateSystemHandle, definition : string) =
    let mutable handle = handle

    override x.ToString() =
        sprintf "CoordinateSystem[%A]" definition

    member x.Definition = definition
    member x.Handle = handle

    member private x.Dispose(disposing : bool) =
        if disposing then GC.SuppressFinalize x
        if handle.IsValid then
            let h = handle
            parent.Start (fun () -> ProjRaw.pDestroyCoordinateSystem h; -1)
            handle <- ProjRaw.CoordinateSystemHandle.Null

    member x.Dispose() = x.Dispose true
    override x.Finalize() = x.Dispose false

    interface IDisposable with
        member x.Dispose() = x.Dispose()

and Context() as this =

    static do
        match UnpackData.run() with
        | Some path -> ProjRaw.pInit path
        | None -> ()

    let mutable ctx = ProjRaw.pCreateContext()

    
    let cleanup = new System.Collections.Concurrent.BlockingCollection<unit -> int>()
    let mutable refCount = 1
    let mutable isDisposed = false

    let cleanupThread =
        let run() =
            for action in cleanup.GetConsumingEnumerable() do
                let delta = lock this (fun () -> action())
                if delta <> 0 then 
                    let o = Interlocked.Add(&refCount, delta)
                    if o + delta <= 0 then
                        lock this (fun () -> 
                            ProjRaw.pDestroyContext ctx
                            ctx <- ProjRaw.ContextHandle.Null
                            cleanup.CompleteAdding()
                        )

        let start = System.Threading.ThreadStart run
        let thread = System.Threading.Thread(start, IsBackground = true)
        thread.Start()
        thread

    member internal x.Start(action : unit -> int) =
        cleanup.Add action

    member x.CreateCoordinateSystem(definition : string) = 
        lock x (fun () ->
            if ctx.IsNull || isDisposed then raise <| ObjectDisposedException "Context"
            let h = ProjRaw.pCreateCoordinateSystem(ctx, definition)
            if h.IsNull then 
                failwithf "[Proj] could not create CoordinateSystem for definition: %A" definition 
            else 
                Interlocked.Increment &refCount |> ignore
                new CoordinateSystem(x, h, definition)
        )

    member x.CreateTransformation(src : CoordinateSystem, dst : CoordinateSystem, [<ParamArray>] options : string[]) =
        lock x (fun () ->
            if ctx.IsNull || isDisposed then raise <| ObjectDisposedException "Context"
            if src.Handle.IsNull then failwithf "[Proj] source CoordinateSystem is disposed: %A" src.Definition
            if dst.Handle.IsNull then failwithf "[Proj] destination CoordinateSystem is disposed: %A" src.Definition

            if not (isNull options) && options.Length > 0 then
                let arr = Array.zeroCreate (1 + options.Length)
                try
                    for i in 0 .. options.Length - 1 do
                        arr.[i] <- Marshal.StringToHGlobalAnsi options.[i]

                    use pOptions = fixed arr
                    let h = ProjRaw.pCreateCoordinateTransformation(ctx, src.Handle, dst.Handle, NativePtr.toNativeInt pOptions)
                    if h.IsNull then failwithf "[Proj] could not create transformation from %A to %A with options: %A" src dst options
                    Interlocked.Increment &refCount |> ignore
                    new Transformation(x, h, src, dst)
                finally
                    for a in arr do 
                        if a <> 0n then Marshal.FreeHGlobal a
            else
                let h = ProjRaw.pCreateCoordinateTransformation(ctx, src.Handle, dst.Handle, 0n)
                if h.IsNull then failwithf "[Proj] could not create transformation from %A to %A" src dst
                Interlocked.Increment &refCount |> ignore
                new Transformation(x, h, src, dst)
                
        )

    member private x.Dispose(disposing : bool) =
        if disposing then GC.SuppressFinalize x
        if ctx.IsValid && not isDisposed then 
            isDisposed <- true
            cleanup.Add (fun () -> -1)

    member x.Dispose() = x.Dispose true
    override x.Finalize() = x.Dispose false

    interface IDisposable with
        member x.Dispose() = x.Dispose()


[<AbstractClass; Sealed>]
type Proj private() =

    static let context = new Context()

    static let transformCache =
        ConditionalWeakTable<CoordinateSystem, ConditionalWeakTable<CoordinateSystem, Transformation>>()

    static let getTransform (src : CoordinateSystem) (dst : CoordinateSystem) =
        lock transformCache (fun () ->
            match transformCache.TryGetValue src with
            | (true, t) ->  
                match t.TryGetValue dst with
                | (true, trans) -> trans
                | _ ->
                    let trans = context.CreateTransformation(src, dst)
                    t.Add(dst, trans) |> ignore
                    trans
            | _ ->
                let table = ConditionalWeakTable()
                let trans = context.CreateTransformation(src, dst)
                table.Add(dst, trans) |> ignore
                transformCache.Add(src, table) |> ignore
                trans
        )

    static let gps = lazy (context.CreateCoordinateSystem "+proj=longlat +datum=WGS84 +no_defs +type=crs")

    static member GPS = gps.Value


    static member Define (definition : string) =
        context.CreateCoordinateSystem definition

    static member Transform (src : CoordinateSystem, dst : CoordinateSystem, points : V3d[]) =
        let trans = getTransform src dst
        trans.Transform points

    static member Transform (src : CoordinateSystem, dst : CoordinateSystem, points : V2d[]) =
        let trans = getTransform src dst
        trans.Transform points

    static member Transform (src : CoordinateSystem, dst : CoordinateSystem, point : V3d) =
        let trans = getTransform src dst
        trans.Transform point

    static member Transform (src : CoordinateSystem, dst : CoordinateSystem, point : V2d) =
        let trans = getTransform src dst
        trans.Transform point
        
