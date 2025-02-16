namespace Viva.Playwright

open System
open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling
open Microsoft.Playwright
open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions

/// <summary> Type of browser. </summary>
/// <category> Unions </category>
type TypeOfBrowser =
    /// <summary> Google Chrome browser. </summary>
    | Chrome
    /// <summary> Chromium browser. </summary>
    | Chromium
    /// <summary> Microsoft Edge browser. </summary>
    | MsEdge
    /// <summary> Mozilla Firefox browser. </summary>
    | Firefox
    /// <summary> Webkit browser. </summary>
    | Webkit

    override this.ToString() =
        match this with
        | Chrome -> "Chrome"
        | Chromium -> "Chromium"
        | MsEdge -> "MsEdge"
        | Firefox -> "Firefox"
        | Webkit -> "Webkit"

    /// <summary> Channel if any for the browser. </summary>
    member this.Channel =
        match this with
        | Chrome -> "chrome"
        | MsEdge -> "msedge"
        | Chromium
        | Firefox
        | Webkit -> ""

    /// <summary> Get the browser type. </summary>
    /// <param name="playwright"> The playwright instance. </param>
    /// <returns> The browser type. </returns>
    member this.GetBrowserType(playwright: IPlaywright) =
        match this with
        | Chrome
        | Chromium
        | MsEdge -> playwright.Chromium
        | Firefox -> playwright.Firefox
        | Webkit -> playwright.Webkit

/// <summary> Represents a playwright context. </summary>
/// <category> Context Types </category>
[<NoComparison; NoEquality>]
type PlaywrightContext = {
    /// <summary> Browser represented by this context. </summary>
    Browser: IBrowser
    /// <summary> Browser context represented by this context. </summary>
    BrowserContext: IBrowserContext
    /// <summary> Logger for this context. </summary>
    Logger: ILogger
    /// <summary> Logger factory for this context. </summary>
    LoggerFactory: ILoggerFactory
    /// <summary> Playwright represented by this context. </summary>
    Playwright: IPlaywright
    /// <summary> Type of browser represented by this context. </summary>
    TypeOfBrowser: TypeOfBrowser
}

/// <summary> Represents a page context. </summary>
/// <category> Context Types </category>
[<NoComparison; NoEquality>]
type PageContext = {
    /// <summary> Playwright context. </summary>
    PlaywrightContext: PlaywrightContext
    /// <summary> Logger for this context. </summary>
    Logger: ILogger
    /// <summary> Page represented by this context. </summary>
    Page: IPage
    /// <summary> Uri of the page. </summary>
    Uri: Uri
} with

    /// <summary> Get logger factory. </summary>
    member this.LoggerFactory: ILoggerFactory = this.PlaywrightContext.LoggerFactory

/// <summary> Represents a locator context. </summary>
/// <category> Context Types </category>
[<NoComparison; NoEquality>]
type LocatorContext = {
    PageContext: PageContext
    Logger: ILogger
    Locator: ILocator
    LocatorString: string
} with

    /// <summary> Create a locator context. </summary>
    /// <param name="locator"> The locator. </param>
    /// <param name="context"> The page context. </param>
    /// <param name="locatorSting"> The locator string used for logging. </param>
    static member Create(locator, context, locatorSting) = {
        PageContext = context
        Logger = context.LoggerFactory.CreateLogger<LocatorContext>()
        Locator = locator
        LocatorString = locatorSting
    }