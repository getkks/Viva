namespace Viva.Playwright

open System
open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling
open Microsoft.Playwright
open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions

/// <summary> Represents a page context. </summary>
type PageContext = {
    /// <summary> Playwright context. </summary>
    PlaywrightContext: PlaywrightContext
    /// <summary> Logger for this context. </summary>
    Logger: ILogger
    /// <summary> Page represented by this context. </summary>
    Page: IPage
    /// <summary> Uri of the page. </summary>
    Uri: Uri
} with

    /// <summary> Get logger factory. </summary>
    member this.LoggerFactory = this.PlaywrightContext.LoggerFactory

module PageContext =
    let openPage uri (context: PlaywrightContext) =
        let logger = context.LoggerFactory.CreateLogger<PageContext>()

        Result.tryCatch(fun _ -> context.BrowserContext.NewPageAsync().Await())
        <!> (fun page ->
            page.GotoAsync(uri).AwaitIgnore()

            {
                PlaywrightContext = context
                Logger = logger
                Page = page
                Uri = Uri uri
            }
        )
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to open {page}.", uri))
        |> Result.tee(fun _ -> logger.LogInformation("Opened {page}.", uri))

    let closePage(context: PageContext) =
        let logger = context.Logger

        Result.tryCatch(fun _ ->
            context.Page.CloseAsync().Await()
            context.PlaywrightContext
        )
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to close {page}.", context.Uri))
        |> Result.tee(fun _ -> logger.LogInformation("Closed {page}.", context.Uri))

    let setDefaultTimeOut (timeOut: TimeSpan) (context: PageContext) =
        context.Page.SetDefaultTimeout(timeOut.TotalMilliseconds |> float32)
        context