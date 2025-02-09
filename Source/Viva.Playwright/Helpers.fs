namespace Viva.Playwright

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open Microsoft.Extensions.Logging
open Microsoft.Playwright

open FsToolkit.ErrorHandling

open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions

[<Extension>]
type TimeSpanExtensions =
    [<Extension>]
    static member ToNullableMilliseconds(this: TimeSpan voption) =
        match this with
        | ValueSome timeSpan -> timeSpan.TotalMilliseconds |> float32 |> Nullable
        | ValueNone -> Nullable()

    [<Extension>]
    static member ToNullable(this: 'T voption) =
        match this with
        | ValueSome value -> value |> Nullable
        | ValueNone -> Nullable()

[<AutoOpen>]
module Helpers =
    let timeSpanToMilliseconds(timeSpan: TimeSpan voption) =
        match timeSpan with
        | ValueSome timeSpan -> timeSpan.TotalMilliseconds |> float32 |> Nullable
        | ValueNone -> Nullable()