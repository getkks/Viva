open System
// open Microsoft.Extensions.Logging
// open Microsoft.Playwright

// open FsToolkit.ErrorHandling
// open Viva.Runtime.Helpers
// open Viva.Runtime.Helpers.Operator.Result
// open Viva.Runtime.Extensions
// open Viva.Playwright

// let factory =
//     LoggerFactory.Create(fun builder ->
//         builder
//             .AddSimpleConsole(fun options ->
//                 options.SingleLine <- true
//                 options.IncludeScopes <- true
//                 options.TimestampFormat <- "hh:mm:ss tt - "
//             )
//             .SetMinimumLevel
// #if DEBUG
//             LogLevel.Debug
// #else
//             LogLevel.Information
// #endif

//         |> ignore
//     )

// MsEdge.Create(factory, BrowserTypeLaunchOptions(Headless = false), BrowserNewContextOptions(ViewportSize = ViewportSize.NoViewport))
// >>= _.Open("https://www.youtube.com/")
// <!> _.SetDefaultTimeOut(TimeSpan.FromMinutes 30L)
// <!> _.GetByRole(AriaRole.Combobox, "Search")
// >>= _.Fill("linux brtfs")
// >>= _.Press("ArrowDown")
// >>= _.PressAndClose("Enter")
// <!> _.GetByLabel("Search filters")
// >>= _.ClickAndClose()
// <!> _.GetBySelector("ytd-search-filter-renderer", "This year")
// >>= _.ClickAndClose()
// <!> _.GetByRole(AriaRole.Link, "This year")
// >>= _.ClickAndClose()
// |> Result.tee(fun _ -> Async.Sleep 5000 |> Async.RunSynchronously)
// >>= _.CloseAll()
// |> ignore

// factory.Dispose()
open Avalonia

[<CompiledName "BuildAvaloniaApp">]
let buildAvaloniaApp() =
    AppBuilder.Configure<Viva.UI.HostApplication>().UsePlatformDetect().WithInterFont().UseSkia().LogToTrace(areas = Array.empty)

[<EntryPoint>]
let main args =
    buildAvaloniaApp().StartWithClassicDesktopLifetime args
// Viva.UI.Program.main([||]) |> ignore