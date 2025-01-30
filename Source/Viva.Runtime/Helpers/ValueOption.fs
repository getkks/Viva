namespace Viva.Runtime.Helpers

open System
open System.Net
open System.Runtime.CompilerServices
open FsToolkit.ErrorHandling

module ValueOption =

    /// <summary> Shorthand for <c>ValueOption.map</c> </summary>
    /// <param name="mapper">The function to map over the <c>ValueOption</c> value.</param>
    /// <param name="input">The <c>ValueOption</c> to map over.</param>
    /// <returns>The result of mapping the function over the <c>ValueOption</c> value.</returns>
    let inline (<!>) (input: 'T voption) (([<InlineIfLambda>] mapper: 'T -> 'R)) : 'R voption = ValueOption.map mapper input

    /// <summary> Shorthand for <c>ValueOption.bind</c> </summary>
    /// <param name="input">The <c>ValueOption</c> bind over.</param>
    /// <param name="binder">The function to bind over the <c>ValueOption</c> value.</param>
    /// <returns>The result of binding the function over the <c>ValueOption</c> value.</returns>
    let inline (>>=) (input: 'T voption) ([<InlineIfLambda>] binder: 'T -> 'R voption) : 'R voption = ValueOption.bind binder input

    /// <summary> Call the function in <c>ValueOption</c> using the value from another <c>ValueOption</c>.</summary>
    /// <param name="applier">The <c>ValueOption</c> containing the function to apply.</param>
    /// <param name="input">The <c>ValueOption</c> value containing the value to apply the function to.</param>
    /// <returns>The result of applying the function in the <c>ValueOption</c> to the value in the other <c>ValueOption</c> value.</returns>
    let inline (<*>) (input: 'T voption) (applier: ValueOption<'T -> 'R>) : 'R voption =
        input
        |> ValueOption.bind(fun value -> applier |> ValueOption.map(fun appplier -> appplier value))

    /// <summary> Perfrom HTML decoding on the <paramref name="value"/> passed. </summary>
    /// <param name="value">The value to decode.</param>
    /// <returns>The decoded value as a <c>ValueOption</c>.</returns>
    let inline htmlDecode(value: string voption) =
        value |> ValueOption.map WebUtility.HtmlDecode

    /// <summary> Perfrom URL decoding on the <paramref name="value"/> passed. </summary>
    /// <param name="value">The value to decode.</param>
    /// <returns>The decoded value as a <c>ValueOption</c>.</returns>
    let inline uriDecode(value: string voption) =
        value |> ValueOption.map WebUtility.UrlDecode