(**
# Example using Viva.Playwright

This examples demonstrates how to use Viva.Playwright to interact with a website.

-  Opens youtube.com
-  Sets a default timeout of 30 minutes
-  Fills the search box with "linux"
-  Presses the arrow down key
-  Presses the enter key
-  Clicks the "Search filters" button
-  Clicks the "This year" filter
-  Closes the browser
*)
(*** hide ***)
#r "/tmp/Artifacts/bin/Viva.Playwright/debug/Viva.Playwright.dll"
#r "/tmp/Artifacts/bin/Viva.Runtime/debug/Viva.Runtime.dll"
#r "nuget: Microsoft.Extensions.Logging.Console"
#r "nuget: Microsoft.Playwright"
#r "nuget: FsToolkit.ErrorHandling"
(**
*)
open System
open Microsoft.Extensions.Logging
open Microsoft.Playwright

open FsToolkit.ErrorHandling
open Viva.Runtime.Helpers.Operator.Result
open Viva.Playwright

let factory =
    LoggerFactory.Create(fun builder ->
        builder
            .AddSimpleConsole(fun options ->
                options.SingleLine <- true
                options.IncludeScopes <- true
                options.TimestampFormat <- "hh:mm:ss tt - "
            )
            .SetMinimumLevel
#if DEBUG
            LogLevel.Debug
#else
            LogLevel.Information
#endif

        |> ignore
    )

MsEdge.Create(factory, BrowserTypeLaunchOptions(Headless = false), BrowserNewContextOptions(ViewportSize = ViewportSize.NoViewport))
>>= _.Open("https://www.youtube.com/")
<!> _.SetDefaultTimeOut(TimeSpan.FromMinutes 30L)
<!> _.GetByRole(AriaRole.Combobox, "Search")
>>= _.Fill("linux brtfs")
>>= _.Press("ArrowDown")
>>= _.PressAndClose("Enter")
<!> _.GetByLabel("Search filters")
>>= _.ClickAndClose()
<!> _.GetBySelector("ytd-search-filter-renderer", "This year")
>>= _.ClickAndClose()
<!> _.GetByRole(AriaRole.Link, "This year")
>>= _.ClickAndClose()
|> Result.tee(fun _ -> Async.Sleep 5000 |> Async.RunSynchronously)
>>= _.CloseAll()
|> ignore

factory.Dispose()
(**
## Console Output
*)
(*** include-fsi-merged-output ***)