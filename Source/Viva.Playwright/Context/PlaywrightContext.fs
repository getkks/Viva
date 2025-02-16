namespace Viva.Playwright

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling
open Microsoft.Playwright
open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions
open Microsoft.FSharp.Core

/// <summary> Extensions for <see cref="PlaywrightContext"/>. </summary>
/// <category> Extensions </category>
[<Extension>]
type PlaywrightContextExtensions =

    /// <summary> Close the <see cref="PlaywrightContext"/>. </summary>
    /// <param name="context"> The <see cref="PlaywrightContext"/>. </param>
    /// <returns> Returns <c>unit</c> if successful; otherwise, the error. </returns>
    [<Extension>]
    static member Close(context: PlaywrightContext) =
        let logger = context.Logger
        let browserContext = context.BrowserContext
        let browser = context.Browser

        result {
            do!
                browserContext.DisposeAsync().AwaitResult()
                >>= (fun _ -> browserContext.CloseAsync().AwaitResult())
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to close browser context.\n{Error}", ex.Message))

            logger.LogInformation "Closed browser context."

            do!
                browser.DisposeAsync().AwaitResult()
                >>= (fun _ -> browser.CloseAsync().AwaitResult())
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to close {browser}.", context.TypeOfBrowser))

            logger.LogInformation("Closed {browser}.", context.TypeOfBrowser)

            do!
                Result.tryCatch(fun _ -> context.Playwright.Dispose())
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to close Playwright context. Error: {Error}", ex.Message))

            logger.LogInformation "Closed Playwright contextare also to successfully."
            context.LoggerFactory.Dispose()
        }

    /// <summary> Create a new <see cref="PlaywrightContext"/>. </summary>
    /// <param name="typeOfBrowser"> The type of browser to create. </param>
    /// <param name="factory"> The logger factory. </param>
    /// <param name="launchOptions"> The launch options. </param>
    /// <param name="newContextOptions"> The new context options. </param>
    /// <returns> The <see cref="PlaywrightContext"/>. </returns>
    [<Extension>]
    static member Create
        (
            typeOfBrowser: TypeOfBrowser,
            factory: ILoggerFactory,
            [<Optional; DefaultParameterValue(null: BrowserTypeLaunchOptions | null)>] launchOptions: BrowserTypeLaunchOptions | null,
            [<Optional; DefaultParameterValue(null: BrowserNewContextOptions | null)>] newContextOptions: BrowserNewContextOptions | null
        ) =
        let logger = factory.CreateLogger<PlaywrightContext>()

        result {
            let! playwright =
                Playwright.CreateAsync().AwaitResult()
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to create Playwright instance."))

            logger.LogInformation "Created Playwright instance successfully."
            let browserType = typeOfBrowser.GetBrowserType playwright

            let launchOptions =
                match launchOptions with
                | null -> BrowserTypeLaunchOptions()
                | launchOptions -> launchOptions

            launchOptions.Channel <- typeOfBrowser.Channel

            let! browser =
                (fun _ -> browserType.LaunchAsync(launchOptions).AwaitResult())
                |> Result.retryAfter(fun _ -> typeOfBrowser.InstallBrowser logger)
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to create {browser}.", typeOfBrowser))

            logger.LogInformation("Created {browser}.", typeOfBrowser)

            let! context =
                browser.NewContextAsync(newContextOptions).AwaitResult()
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to create context for {browser}.", typeOfBrowser))

            logger.LogInformation("Created context for {browser}.", typeOfBrowser)

            return {
                Browser = browser
                BrowserContext = context
                Logger = logger
                LoggerFactory = factory
                Playwright = playwright
                TypeOfBrowser = typeOfBrowser
            }
        }

    [<Extension>]
    static member private InstallBrowser(typeOfBrowser: TypeOfBrowser, logger: ILogger) =
        logger.LogInformation("Installing {browser}...", typeOfBrowser)

        if
            Program.Main [|
                "install"
                typeOfBrowser.Channel
            |] = 0
        then
            logger.LogInformation("{browser} installed successfully.", typeOfBrowser)
            Ok()
        else
            logger.LogError("Failed to install {browser}.", typeOfBrowser)
            "Failed to install the browser." |> exn |> Error