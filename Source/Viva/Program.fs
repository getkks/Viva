open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling
open Microsoft.Playwright
open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions

type TypeOfBrowser =
    | Chrome
    | Chromium
    | MsEdge
    | Firefox
    | Webkit

    override this.ToString() =
        match this with
        | Chrome -> "Chrome"
        | Chromium -> "Chromium"
        | MsEdge -> "MsEdge"
        | Firefox -> "Firefox"
        | Webkit -> "Webkit"

    member this.Channel =
        match this with
        | Chrome -> "chrome"
        | MsEdge -> "msedge"
        | Chromium
        | Firefox
        | Webkit -> ""

type PlaywrightContext = {
    Browser: IBrowser
    BrowserContext: IBrowserContext
    LoggerFactory: ILoggerFactory
    Page: IPage voption
    Playwright: IPlaywright
    TypeOfBrowser: TypeOfBrowser
}

let createPlaywrightContext(factory: ILoggerFactory) =
    let logger = factory.CreateLogger<PlaywrightContext>()

    Result.tryCatch(fun _ -> Playwright.CreateAsync().Await())
    |> Result.tee(fun _ -> logger.LogInformation "Created Playwright instance successfully.")
    |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to create Playwright instance. Error: {Error}", ex.Message))
    <!> (fun play -> {|
        Playwright = play
        LoggerFactory = factory
    |})

let install (typeOfBrowser: TypeOfBrowser) (logger: ILogger) =
    result {
        logger.LogInformation("Installing {browser}...", typeOfBrowser)

        let! _ =
            Result.requireEqual
                (Program.Main [|
                    "install"
                    typeOfBrowser.Channel
                |])
                0
                (exn "Failed to install the browser.")

        ()
    }
    |> Result.tee(fun _ -> logger.LogInformation "Browser installed.")
    |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to install the {browser}.", typeOfBrowser))

let createBrowser
    typeOfBrowser
    (options: BrowserTypeLaunchOptions)
    (seed:
        {|
            LoggerFactory: ILoggerFactory
            Playwright: IPlaywright
        |})
    =
    let logger = seed.LoggerFactory.CreateLogger<PlaywrightContext>()

    let browserType =
        match typeOfBrowser with
        | Chrome
        | Chromium
        | MsEdge -> seed.Playwright.Chromium
        | Firefox -> seed.Playwright.Firefox
        | Webkit -> seed.Playwright.Webkit

    options.Channel <-
        match typeOfBrowser with
        | Chrome -> "chrome"
        | Chromium -> "chromium"
        | MsEdge -> "msedge"
        | Firefox
        | Webkit -> ""

    Result.tryCatch(fun _ -> browserType.LaunchAsync(options).Await())
    |> Result.orElseWith(fun ex ->
        install typeOfBrowser logger
        |> Result.mapTryCatch(fun _ -> browserType.LaunchAsync(options).Await())
    )
    |> Result.tee(fun _ -> logger.LogInformation("Created {browser}.", typeOfBrowser))
    |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to create {browser}.", typeOfBrowser))
    <!> (fun browser -> {|
        Browser = browser
        LoggerFactory = seed.LoggerFactory
        Playwright = seed.Playwright
        TypeOfBrowser = typeOfBrowser
    |})

let createContext
    (options: BrowserNewContextOptions)
    (seed:
        {|
            Browser: IBrowser
            LoggerFactory: ILoggerFactory
            Playwright: IPlaywright
            TypeOfBrowser: TypeOfBrowser
        |})
    =
    let logger = seed.LoggerFactory.CreateLogger<PlaywrightContext>()

    seed.Browser.NewContextAsync(options).AwaitResult()
    |> Result.tee(fun _ -> logger.LogDebug("Created context for {browser}.", seed.TypeOfBrowser))
    |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to create context for {browser}.", seed.TypeOfBrowser))
    <!> (fun context -> {
        Browser = seed.Browser
        BrowserContext = context
        LoggerFactory = seed.LoggerFactory
        Page = ValueNone
        Playwright = seed.Playwright
        TypeOfBrowser = seed.TypeOfBrowser
    })

let closeContext(context: PlaywrightContext) =
    let logger = context.LoggerFactory.CreateLogger<PlaywrightContext>()
    let browserContext = context.BrowserContext
    let browser = context.Browser

    result {
        do!
            browserContext.DisposeAsync().AwaitResult()
            >>= (fun _ -> browserContext.CloseAsync().AwaitResult())
            |> Result.tee(fun _ -> logger.LogInformation("Closed browser context '{browserContext}'.", browserContext))
            |> Result.teeError(fun ex ->
                logger.LogError(ex, "Failed to close browser context '{browserContext}'. Error: {Error}", browserContext, ex.Message)
            )

        do!
            browser.DisposeAsync().AwaitResult()
            >>= (fun _ -> browser.CloseAsync().AwaitResult())
            |> Result.tee(fun _ -> logger.LogInformation("Closed {browser}.", context.TypeOfBrowser))
            |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to close {browser}.", context.TypeOfBrowser))

        do!
            Result.tryCatch(fun _ -> context.Playwright.Dispose())
            |> Result.tee(fun _ -> logger.LogInformation "Closed Playwright context successfully.")
            |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to close Playwright context. Error: {Error}", ex.Message))

        context.LoggerFactory.Dispose()
    }

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
|> createPlaywrightContext
>>= createBrowser MsEdge (BrowserTypeLaunchOptions(Headless = false))
>>= createContext(BrowserNewContextOptions(ViewportSize = ViewportSize.NoViewport))
>>= closePage
<!> (fun context ->
    [
        "https://playwright.dev/"
        "https://nuget.org/"
    ]
    |> Seq.iter(fun uri -> context |> openPage uri >>= closePage |> ignore)

    context
)
>>= closeContext
|> ignore

factory.Dispose()