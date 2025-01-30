namespace Viva.Playwright.Helpers

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading.Tasks
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Microsoft.Playwright
open IcedTasks
open FsToolkit.ErrorHandling
open Viva.Runtime.Helpers
open Viva.Runtime.Extensions

[<Extension; Sealed; AbstractClass>]
type BrowserTypeExtensions =
    [<Extension>]
    static member Install(this: IBrowserType, [<Optional>] options: BrowserTypeLaunchOptions, [<Optional>] logger: ILogger) =
        this.Install(options, logger |> ValueOption.ofObj)

    [<Extension>]
    static member Install(this: IBrowserType, [<Optional>] options: BrowserTypeLaunchOptions, [<Optional>] logger: ILogger voption) =
        logger
        |> ValueOption.iter(fun logger -> logger.LogInformation "Installing the browser...")

        result {
            if
                [|
                    "install"
                    if this.Name <> BrowserType.Chromium then
                        this.Name
                    else
                        match options.Channel with
                        | NonNull channel -> channel
                        | _ -> "chrome"
                |]
                |> Program.Main
                <> 0
            then
                let msg = "Failed to install the browser."
                logger |> ValueOption.iter(fun logger -> logger.LogError msg)
                return! msg |> exn |> Error
            else
                logger
                |> ValueOption.iter(fun logger -> logger.LogInformation "Browser installed.")
        }

    [<Extension>]
    static member Launch(this: IBrowserType, [<Optional>] options: BrowserTypeLaunchOptions, [<Optional>] logger: ILogger) =
        this.Launch(options, logger |> ValueOption.ofObj)

    [<Extension>]
    static member Launch(this: IBrowserType, [<Optional>] options: BrowserTypeLaunchOptions, [<Optional>] logger: ILogger voption) =
        result {
            let res = Result.tryCatch(fun () -> this.LaunchAsync(options).Await())

            match res with
            | Error ex when ex.Message.Contains "Executable doesn't exist at" ->
                this.Install(options, logger)

                logger
                |> ValueOption.iter(fun logger -> logger.LogInformation "Installing the browser...")

                if
                    [|
                        "install"
                        if this.Name <> BrowserType.Chromium then
                            this.Name
                        else
                            match options.Channel with
                            | NonNull channel -> channel
                            | _ -> "chrome"
                    |]
                    |> Program.Main
                    <> 0
                then
                    logger
                    |> ValueOption.iter(fun logger -> logger.LogError "Failed to install the browser.")

                    return! res
                else
                    logger
                    |> ValueOption.iter(fun logger -> logger.LogInformation "Browser installed.")

                    return! Result.tryCatch(fun () -> this.LaunchAsync(options).Await())
            | _ -> return! res
        }
        |> Result.tee(fun _ -> logger |> ValueOption.iter _.LogDebug("Browser instance created."))
        |> Result.teeError(fun ex -> logger |> ValueOption.iter _.LogError(ex, "Failed to create browser instance."))

[<Extension; Sealed; AbstractClass>]
type Playwright =
    static member Create([<Optional>] logger: ILogger) =
        let logger = logger |> ValueOption.ofObj

        Result.tryCatch(fun () -> Playwright.CreateAsync().Await())
        |> Result.tee(fun _ -> logger |> ValueOption.iter _.LogDebug("Playwright instance created."))
        |> Result.teeError(fun ex ->
            logger
            |> ValueOption.iter _.LogError(ex, "Failed to create Playwright instance.")
        )

    [<Extension>]
    static member LaunchEdge(this: IPlaywright, [<Optional>] options: BrowserTypeLaunchOptions | null, [<Optional>] logger: ILogger) =
        this.Chromium.Launch(
            (match options with
             | NonNull options ->
                 options.Channel <- "msedge"
                 options
             | _ -> BrowserTypeLaunchOptions(Channel = "msedge")),
            logger
        )