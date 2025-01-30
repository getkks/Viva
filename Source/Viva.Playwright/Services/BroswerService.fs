namespace Viva.Playwright.Services

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Playwright
open IcedTasks
open FsToolkit.ErrorHandling
open FsToolkit.ErrorHandling.Operator.Result
open Viva.Runtime.Helpers
open Viva.Runtime.Extensions

[<AbstractClass>]
type BrowserService(browserType: IBrowserType, options: BrowserTypeLaunchOptions, logger: ILogger) =
    let browserName =
        if browserType.Name <> BrowserType.Chromium then
            browserType.Name
        else
            match options.Channel with
            | NonNull channel -> channel
            | _ -> "chrome"

    let install() =
        result {
            logger.LogInformation("Installing {browser}...", browserName)

            let! _ =
                Result.requireEqual
                    (Program.Main [|
                        "install"
                        browserName
                    |])
                    0
                    (exn "Failed to install the browser.")

            ()
        }
        |> Result.tee(fun _ -> logger.LogInformation "Browser installed.")
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to install the {browser}.", browserName))

    let browser =
        Result.tryCatch(fun _ -> browserType.LaunchAsync(options).Await())
        |> Result.bindOnError(fun ex ->
            install()
            >>= (fun _ -> Result.tryCatch(fun _ -> browserType.LaunchAsync(options).Await()))
        )
        |> Result.tee(fun _ -> logger.LogInformation "Browser launched.")
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to launch the {browser}.", browserName))
        |> Result.raise

    let context =
        let options = BrowserNewContextOptions(ColorScheme = ColorScheme.Dark, ViewportSize = ViewportSize.NoViewport)
        browser.NewContextAsync(options).Await()

    member val Browser = browser
    member val Context = context