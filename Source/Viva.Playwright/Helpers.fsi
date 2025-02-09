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

[<Class>]
[<Extension>]
type TimeSpanExtensions =
    [<Extension>]
    static member ToNullableMilliseconds: this: TimeSpan voption -> Nullable<float32>

    [<Extension>]
    static member ToNullable: this: 'T voption -> Nullable<'T>

[<AutoOpen>]
module Helpers =
    val timeSpanToMilliseconds: timeSpan: TimeSpan voption -> Nullable<float32>