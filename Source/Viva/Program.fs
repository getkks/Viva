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

PlaywrightContext.create(
    MsEdge,
    BrowserTypeLaunchOptions(Headless = false),
    BrowserNewContextOptions(ViewportSize = ViewportSize.NoViewport),
    factory
)
>>= PageContext.openPage "https://www.youtube.com/"
<!> PageContext.setDefaultTimeOut(System.TimeSpan.FromSeconds(30L))
|> Result.tee(fun pageContext ->
    pageContext
    |> LocatorContext.getByRole AriaRole.Combobox "Search" ValueNone
    |> LocatorContext.fill "linux brtfs vs " ValueNone
    >>= LocatorContext.press "ArrowDown" ValueNone ValueNone
    >>= LocatorContext.press "Enter" ValueNone ValueNone
    <!> LocatorContext.ignoreActiveLocator
    >>= LocatorContext.clickByLabel "Search filters" ValueNone ValueNone
    >>= LocatorContext.clickBySelector "ytd-search-filter-renderer" (ValueSome "This year") ValueNone ValueNone
    >>= LocatorContext.clickByRole AriaRole.Link "This year" ValueNone ValueNone
    |> ignore

    Async.Sleep 5000 |> Async.RunSynchronously
)
>>= PageContext.closePage
>>= PlaywrightContext.closeContext
|> ignore

factory.Dispose()