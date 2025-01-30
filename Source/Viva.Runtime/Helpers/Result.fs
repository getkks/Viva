namespace Viva.Runtime.Helpers

open FsToolkit.ErrorHandling
open Microsoft.Extensions.Logging

module Result =
    let inline flatten value =
        match value with
        | Ok v
        | Error(Ok v) -> Ok v
        | Error(Error ex) -> Error ex

    let inline bindOnError binding (value) : Result<'TOut, 'TError> =
        match value with
        | Ok v -> Ok v
        | Error ex -> binding ex

    /// <summary> Wraps a function in a <c>try/catch</c> block. </summary>
    /// <param name="f">The function to wrap in a <c>try/catch</c> block.</param>
    /// <param name="value">The value to pass to the function.</param>
    /// <returns>A <c>Result</c> containing the result of the function, or an error if the function throws an exception.</returns>
    /// <typeparam name="'T">The return type of the function.</typeparam>
    /// <typeparam name="'TError">The type of the exception to catch. This can help filter exceptions.</typeparam>
    /// <typeparam name="'TOut"></typeparam>
    let inline bindTryCatch<'T, 'TError, 'TOut when 'TError :> exn> (f: 'T -> Result<'TOut, 'TError>) value : Result<'TOut, 'TError> =
        try
            Result.bind f value
        with :? 'TError as ex ->
            Error ex

    /// <summary> Wraps a function in a <c>try/catch</c> block. </summary>
    /// <param name="f">The function to wrap in a <c>try/catch</c> block.</param>
    /// <param name="value">The value to pass to the function.</param>
    /// <returns>A <c>Result</c> containing the result of the function, or an error if the function throws an exception.</returns>
    /// <typeparam name="'T">The return type of the function.</typeparam>
    /// <typeparam name="'TError">The type of the exception to catch. This can help filter exceptions.</typeparam>
    /// <typeparam name="'TOut"></typeparam>
    let inline mapTryCatch<'T, 'TError, 'TOut when 'TError :> exn> (f: 'T -> 'TOut) value : Result<'TOut, 'TError> =
        try
            Result.map f value
        with :? 'TError as ex ->
            Error ex

    /// <summary> Wraps a function in a <c>try/catch</c> block. </summary>
    /// <param name="f">The function to wrap in a <c>try/catch</c> block.</param>
    /// <returns>A <c>Result</c> containing the result of the function, or an error if the function throws an exception.</returns>
    /// <typeparam name="'T">The return type of the function.</typeparam>
    /// <typeparam name="'TError">The type of the exception to catch. This can help filter exceptions.</typeparam>
    let inline tryCatch<'T, 'TError when 'TError :> exn>(f: unit -> 'T) : Result<'T, 'TError> =
        try
            Ok(f())
        with :? 'TError as ex ->
            Error ex

    let inline raise value =
        match value with
        | Error ex -> raise ex
        | Ok v -> v

    /// <summary> Requires a value to be <c>ValueSome</c>, otherwise returns an error result. </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="errorMessage">The error message to return if the value is <c>ValueNone</c>.</param>
    /// <returns>An <c>Ok</c> result if the value is <c>ValueSome</c>, otherwise an <c>Error</c> result with the specified error message.</returns>
    let requireValueSomeWithError errorMessage (value: 'T voption) : Result<'T, exn> =
        value |> Result.requireValueSome(exn errorMessage)

    /// <summary> Requires a value to be <c>NonNull</c>, otherwise returns an error result. </summary>
    /// <param name="error">The error value to return if the value is <c>null</c>.</param>
    /// <param name="nullable">The value to check.</param>
    /// <returns>An <c>Ok</c> result if the value is <c>NonNull</c>, otherwise an <c>Error</c> result with the specified error value.</returns>
    let inline requireNonNull (error: 'TError) (nullable: 'T | null) : Result<'T, 'TError> =
        match nullable with
        | NonNull x -> Ok x
        | _ -> Error error

module Operator =
    module Result =

        /// <summary>
        /// Shorthand for <c>Result.map</c>
        /// </summary>
        /// <param name="mapper">The function to map over the <c>Result</c> value.</param>
        /// <param name="input">The <c>Result</c> value to map over.</param>
        /// <returns>The result of mapping the function over the <c>Result</c> value.</returns>
        let inline (<!>)
            (input: Result<'okInput, 'error>)
            (([<InlineIfLambda>] mapper: 'okInput -> 'okOutput))
            : Result<'okOutput, 'error> =
            Result.map mapper input

        /// <summary>
        /// Shorthand for <c>Result.apply</c>
        /// </summary>
        /// <param name="applier">The <c>Result</c> value containing the function to apply.</param>
        /// <param name="input">The <c>Result</c> value containing the value to apply the function to.</param>
        /// <returns>The result of applying the function in the <c>Result</c> value to the value in the other <c>Result</c> value.</returns>
        let inline (<*>) (input: Result<'okInput, 'error>) (applier: Result<'okInput -> 'okOutput, 'error>) : Result<'okOutput, 'error> =
            Result.apply applier input

        /// <summary>
        /// Shorthand for <c>Result.bind</c>
        /// </summary>
        /// <param name="input">The <c>Result</c> value to bind over.</param>
        /// <param name="binder">The function to bind over the <c>Result</c> value.</param>
        /// <returns>The result of binding the function over the <c>Result</c> value.</returns>
        let inline (>>=)
            (input: Result<'input, 'error>)
            ([<InlineIfLambda>] binder: 'input -> Result<'okOutput, 'error>)
            : Result<'okOutput, 'error> =
            Result.bind binder input