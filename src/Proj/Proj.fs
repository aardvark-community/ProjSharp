namespace Proj

open System
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open Microsoft.FSharp.NativeInterop
open Aardvark.Base

#nowarn "9"



module ProjRaw =

    type ContextHandle =
        struct
            val mutable public Handle : nativeint

            member x.IsNull = x.Handle = 0n
            member x.IsValid = x.Handle <> 0n
            static member Null = ContextHandle(Handle = 0n)
        end


    type TransformationHandle =
        struct
            val mutable public Handle : nativeint
            member x.IsNull = x.Handle = 0n
            member x.IsValid = x.Handle <> 0n
            static member Null = TransformationHandle(Handle = 0n)
        end

    [<DllImport("ProjNative")>]
    extern ContextHandle pCreateContext();
    
    [<DllImport("ProjNative")>]
    extern void pDestroyContext(ContextHandle context);

    [<DllImport("ProjNative")>]
    extern TransformationHandle pCreateTransformation(ContextHandle context, [<MarshalAs(UnmanagedType.LPStr)>] string src, [<MarshalAs(UnmanagedType.LPStr)>] string dst)

    [<DllImport("ProjNative")>]
    extern void pDestroyTransformation(TransformationHandle trans)

    [<DllImport("ProjNative")>]
    extern bool pTransform(TransformationHandle trans, int count, V3d* inPoints, V3d* outPoints)


type Transformation(parent : Proj, handle : ProjRaw.TransformationHandle, src : string, dst : string) =
    let mutable handle = handle
 
    member x.Transform(points : V3d[]) = 
        lock parent (fun () ->
            let res = Array.zeroCreate points.Length
            use src = fixed points
            use dst = fixed res
            if ProjRaw.pTransform(handle, points.Length, src, dst) then
                res
            else
                failwith "could not transform"
        )

    member private x.Dispose(disposing : bool) =
        if disposing then GC.SuppressFinalize x
        if handle.IsValid then
            ProjRaw.pDestroyTransformation handle
            handle <- ProjRaw.TransformationHandle.Null

    member x.Dispose() = x.Dispose true
    override x.Finalize() = x.Dispose false

    interface IDisposable with
        member x.Dispose() = x.Dispose()
and Proj() =
    let mutable ctx = ProjRaw.pCreateContext()

    member x.CreateTransformation(src : string, dst : string) =
        lock x (fun () ->
            let h = ProjRaw.pCreateTransformation(ctx, src, dst)
            new Transformation(x, h, src, dst)
        )

    member private x.Dispose(disposing : bool) =
        if disposing then GC.SuppressFinalize x
        if ctx.IsValid then
            ProjRaw.pDestroyContext ctx
            ctx <- ProjRaw.ContextHandle.Null

    member x.Dispose() = x.Dispose true
    override x.Finalize() = x.Dispose false

    interface IDisposable with
        member x.Dispose() = x.Dispose()