namespace Viva.Playwright

open System
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

    member this.GetBrowserType(playwright: IPlaywright) =
        match this with
        | Chrome
        | Chromium
        | MsEdge -> playwright.Chromium
        | Firefox -> playwright.Firefox
        | Webkit -> playwright.Webkit

type PlaywrightContext = {
    Browser: IBrowser
    BrowserContext: IBrowserContext
    Logger: PlaywrightContext ILogger
    LoggerFactory: ILoggerFactory
    Playwright: IPlaywright
    TypeOfBrowser: TypeOfBrowser
}

module PlaywrightContext =
    let internal createPlaywright(factory: ILoggerFactory) =
        let logger = factory.CreateLogger<PlaywrightContext>()

        Playwright.CreateAsync().AwaitResult()
        |> Result.tee(fun _ -> logger.LogInformation "Created Playwright instance successfully.")
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to create Playwright instance."))
        <!> (fun play -> {|
            Playwright = play
            LoggerFactory = factory
        |})

    let internal install (typeOfBrowser: TypeOfBrowser) (logger: ILogger) =
        result {
            logger.LogInformation("Installing {browser}...", typeOfBrowser)

            do!
                Result.requireEqual
                    (Program.Main [|
                        "install"
                        typeOfBrowser.Channel
                    |])
                    0
                    (exn "Failed to install the browser.")

            ()
        }
        |> Result.tee(fun _ -> logger.LogInformation("{browser} installed.", typeOfBrowser))
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to install {browser}."))

    let internal createBrowser
        (typeOfBrowser: TypeOfBrowser)
        (options: BrowserTypeLaunchOptions)
        (seed:
            {|
                LoggerFactory: ILoggerFactory
                Playwright: IPlaywright
            |})
        =
        let logger = seed.LoggerFactory.CreateLogger<PlaywrightContext>()
        let browserType = typeOfBrowser.GetBrowserType seed.Playwright

        let options =
            match options with
            | NonNull options ->
                options.Channel <- typeOfBrowser.Channel
                options
            | _ -> BrowserTypeLaunchOptions(Channel = typeOfBrowser.Channel)

        browserType.LaunchAsync(options).AwaitResult()
        |> Result.orElseWith(fun _ ->
            result {
                do! install typeOfBrowser logger
                return! browserType.LaunchAsync(options).AwaitResult()
            }
        )
        |> Result.tee(fun _ -> logger.LogInformation("Created {browser}.", typeOfBrowser))
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to create {browser}.", typeOfBrowser))
        <!> (fun browser -> {|
            Browser = browser
            Logger = logger
            LoggerFactory = seed.LoggerFactory
            Playwright = seed.Playwright
            TypeOfBrowser = typeOfBrowser
        |})

    let internal createContext
        (options: BrowserNewContextOptions)
        (seed:
            {|
                Browser: IBrowser
                Logger: PlaywrightContext ILogger
                LoggerFactory: ILoggerFactory
                Playwright: IPlaywright
                TypeOfBrowser: TypeOfBrowser
            |})
        =
        let logger = seed.Logger

        seed.Browser.NewContextAsync(options).AwaitResult()
        |> Result.tee(fun _ -> logger.LogDebug("Created context for {browser}.", seed.TypeOfBrowser))
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to create context for {browser}.", seed.TypeOfBrowser))
        <!> (fun context -> {
            Browser = seed.Browser
            BrowserContext = context
            Logger = logger
            LoggerFactory = seed.LoggerFactory
            Playwright = seed.Playwright
            TypeOfBrowser = seed.TypeOfBrowser
        })

    let create(typeOfBrowser, launchOptions, newContextOptions, factory) =
        factory
        |> createPlaywright
        >>= createBrowser typeOfBrowser launchOptions
        >>= createContext newContextOptions

    let closeContext(context: PlaywrightContext) =
        let logger = context.Logger
        let browserContext = context.BrowserContext
        let browser = context.Browser

        result {
            do!
                browserContext.DisposeAsync().AwaitResult()
                >>= (fun _ -> browserContext.CloseAsync().AwaitResult())
                |> Result.tee(fun _ -> logger.LogInformation("Closed browser context."))
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to close browser context.\n{Error}", ex.Message))

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