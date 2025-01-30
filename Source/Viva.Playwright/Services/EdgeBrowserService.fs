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

type EdgeBrowserService(playService: PlaywrightService, logger: EdgeBrowserService ILogger) =
    inherit BrowserService(playService.Playwright.Chromium, BrowserTypeLaunchOptions(Headless = false, Channel = "msedge"), logger)

    static member Register(services: IServiceCollection) =
        services.AddTransient<BrowserService, EdgeBrowserService>()

[<Extension; AbstractClass; Sealed>]
type EdgeBrowserServiceExtensions =
    [<Extension>]
    static member AddEdgeBrowserService(services: IServiceCollection) = EdgeBrowserService.Register services