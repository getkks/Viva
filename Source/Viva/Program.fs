open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling
open Microsoft.Playwright
open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions
open Viva.Playwright

let factory =
    LoggerFactory.Create(fun builder ->
        builder
            .AddSimpleConsole(fun options ->
                options.SingleLine <- true
                options.IncludeScopes <- true
                options.TimestampFormat <- "hh:mm:ss tt - "
            )
            .SetMinimumLevel(
#if DEBUG
                LogLevel.Debug
#else
                LogLevel.Information
#endif
            )
        |> ignore
    )

let rec openPage uri (context: PlaywrightContext) =
    let logger = context.LoggerFactory.CreateLogger(nameof openPage)

    Result.tryCatch(fun _ -> context.BrowserContext.NewPageAsync().Await())
    |> Result.mapTryCatch(fun page ->
        page.GotoAsync(uri).AwaitIgnore()
        Async.Sleep 2000 |> Async.RunSynchronously
        { context with Page = ValueSome page }
    )
    |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to open {page}.", uri))
    |> Result.tee(fun _ -> logger.LogInformation("Opened {page}.", uri))

let rec closePage(context: PlaywrightContext) =
    let logger = context.LoggerFactory.CreateLogger(nameof closePage)

    match context.Page with
    | ValueNone ->
        logger.LogDebug("No page to close.")
        context |> Ok
    | ValueSome page ->
        let url = page.Url

        Result.tryCatch(fun _ -> page.CloseAsync().Await())
        <!> (fun _ -> { context with Page = ValueNone })
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to close {page}.", url))
        |> Result.tee(fun _ -> logger.LogInformation("Closed {page}.", url))

let closePages(contextSeq: PlaywrightContext seq) =
    contextSeq |> Seq.traverseResultA closePage <!> Seq.head

let openPages uris (context: PlaywrightContext) =
    uris |> Seq.traverseResultA(fun uri -> openPage uri context)

factory
|> PlaywrightContext.createPlaywrightContext
    MsEdge
    (BrowserTypeLaunchOptions(Headless = false))
    (BrowserNewContextOptions(ViewportSize = ViewportSize.NoViewport))
<!> (fun context ->
    [
        "https://playwright.dev/"
        "https://nuget.org/"
    ]
    |> Seq.iter(fun uri -> context |> openPage uri >>= closePage |> ignore)

    context
)
>>= PlaywrightContext.closeContext
|> ignore

factory.Dispose()