namespace Viva.Runtime.Extensions

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading.Tasks
open IcedTasks
open FsToolkit.ErrorHandling
open Viva.Runtime.Helpers

[<Extension; Sealed; AbstractClass>]
type TaskExtensions =
    /// <summary> Runs the task computation and await its result. </summary>
    /// <param name="this">The task computation to run.</param>
    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member Await(this: Task) =
        this |> Async.AwaitTask |> Async.RunSynchronously

    /// <summary> Runs the task computation and await its result. </summary>
    /// <param name="this">The task computation to run.</param>
    /// <returns>The result of the task computation as <c>Result</c> capturing any thrown exception.</returns>
    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member AwaitResult(this: Task) = Result.tryCatch(fun _ -> this.Await())

    /// <summary> Runs the task computation and await its result. </summary>
    /// <param name="this">The task computation to run.</param>
    /// <returns>The result of the task computation.</returns>
    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member Await(this: 'T Task) =
        this |> Async.AwaitTask |> Async.RunSynchronously

    /// <summary> Runs the task computation and await its result. </summary>
    /// <param name="this">The task computation to run.</param>
    /// <returns>The result of the task computation as <c>Result</c> capturing any thrown exception.</returns>
    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member AwaitResult(this: 'T Task) = Result.tryCatch(fun _ -> this.Await())

    /// <summary> Runs the task computation and await its result. </summary>
    /// <param name="this">The task computation to run.</param>
    /// <returns>The result is ignored.</returns>
    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member AwaitIgnore(this: 'T Task) = this.Await() |> ignore

    /// <summary> Runs the task computation and await its result. </summary>
    /// <param name="this">The task computation to run.</param>
    /// <returns>The result is ignored. Captrues any thrown exception.</returns>
    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member AwaitResultIgnore(this: 'T Task) = this.AwaitResult() |> Result.ignore

    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member Await(this: ValueTask) =
        this |> Async.AwaitValueTask |> Async.RunSynchronously

    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member AwaitResult(this: ValueTask) = Result.tryCatch(fun _ -> this.Await())

    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member Await(this: 'T ValueTask) =
        this |> Async.AwaitValueTask |> Async.RunSynchronously

    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member AwaitResult(this: 'T ValueTask) = Result.tryCatch(fun _ -> this.Await())

    /// <summary> Runs the task computation and await its result. </summary>
    /// <param name="this">The task computation to run.</param>
    /// <returns>The result is ignored.</returns>
    [<Extension; MethodImpl(MethodImplOptions.AggressiveOptimization)>]
    static member AwaitIgnore(this: 'T ValueTask) = this.Await() |> ignore