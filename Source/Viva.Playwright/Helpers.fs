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

/// <namespacedoc>
///   <summary> Contains types for working with Playwright. </summary>
/// </namespacedoc>
/// <summary> Extensions for <see cref="TimeSpan"/>. </summary>
/// <category> Extensions </category>
[<Extension>]
type TimeSpanExtensions =

    /// <summary> Convert <see cref="System.TimeSpan.TotalMilliseconds"/> to a nullable <see cref="float"/>. </summary>
    /// <param name="this"> The <see cref="TimeSpan"/> to convert. </param>
    /// <returns> The nullable <see cref="float"/> value of the <see cref="System.TimeSpan.TotalMilliseconds"/>. </returns>
    [<Extension>]
    static member ToNullableMilliseconds(this: TimeSpan voption) =
        match this with
        | ValueSome timeSpan -> timeSpan.TotalMilliseconds |> float32 |> Nullable
        | ValueNone -> Nullable()

    /// <summary> Convert <see cref="System.TimeSpan.TotalMilliseconds"/> to a nullable <see cref="float"/>. </summary>
    /// <param name="this"> The <see cref="TimeSpan"/> to convert. </param>
    /// <returns> The nullable <see cref="float"/> value of the <see cref="System.TimeSpan.TotalMilliseconds"/>. </returns>
    [<Extension>]
    static member ToNullableMilliseconds(this: TimeSpan) =
        let ms = this.TotalMilliseconds

        match ms with
        | 0. -> Nullable()
        | _ -> ms |> float32 |> Nullable