namespace Viva.Playwright

open System
open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling
open Microsoft.Playwright
open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions

/// <summary> Type of browser. </summary>
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

    override ToString: unit -> string
    /// <summary> Channel if any for the browser. </summary>
    member Channel: string
    /// <summary> Get the browser type. </summary>
    /// <param name="playwright"> The playwright instance. </param>
    /// <returns> The browser type. </returns>
    member GetBrowserType: playwright: IPlaywright -> IBrowserType

/// <summary> Represents a playwright context. </summary>
type PlaywrightContext =
    {
        /// <summary> Browser represented by this context. </summary>
        Browser: IBrowser
        /// <summary> Browser context represented by this context. </summary>
        BrowserContext: IBrowserContext
        /// <summary> Logger for this context. </summary>
        Logger: PlaywrightContext ILogger
        /// <summary> Logger factory for this context. </summary>
        LoggerFactory: ILoggerFactory
        /// <summary> Playwright represented by this context. </summary>
        Playwright: IPlaywright
        /// <summary> Type of browser represented by this context. </summary>
        TypeOfBrowser: TypeOfBrowser
    }

module PlaywrightContext =
    val internal createPlaywright:
        factory: ILoggerFactory ->
            Result<
                {| LoggerFactory: ILoggerFactory
                   Playwright: IPlaywright |},
                #exn
             >

    val internal install: typeOfBrowser: TypeOfBrowser -> logger: ILogger -> Result<unit, exn>

    val internal createBrowser:
        typeOfBrowser: TypeOfBrowser ->
        options: BrowserTypeLaunchOptions ->
        seed:
            {| LoggerFactory: ILoggerFactory
               Playwright: IPlaywright |} ->
            Result<
                {| Browser: IBrowser
                   Logger: ILogger<PlaywrightContext>
                   LoggerFactory: ILoggerFactory
                   Playwright: IPlaywright
                   TypeOfBrowser: TypeOfBrowser |},
                exn
             >

    val internal createContext:
        options: BrowserNewContextOptions ->
        seed:
            {| Browser: IBrowser
               Logger: ILogger<PlaywrightContext>
               LoggerFactory: ILoggerFactory
               Playwright: IPlaywright
               TypeOfBrowser: TypeOfBrowser |} ->
            Result<PlaywrightContext, #exn>

    /// <summary> Creates a new playwright context. </summary>
    /// <param name="typeOfBrowser"> Type of browser. </param>
    /// <param name="launchOptions"> Browser launch options. </param>
    /// <param name="newContextOptions"> Browser context options. </param>
    /// <param name="factory"> Logger factory. </param>
    /// <returns> If successful, returns The playwright context else error. </returns>
    val create:
        typeOfBrowser: TypeOfBrowser *
        launchOptions: BrowserTypeLaunchOptions *
        newContextOptions: BrowserNewContextOptions *
        factory: ILoggerFactory ->
            Result<PlaywrightContext, exn>

    /// <summary> Closes the playwright context. </summary>
    /// <param name="context"> The playwright context. </param>
    /// <returns> If successful, returns <c>unit</c> else error. </returns>
    val closeContext: context: PlaywrightContext -> Result<unit, #exn>