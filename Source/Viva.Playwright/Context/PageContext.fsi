namespace Viva.Playwright

open System
open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling
open Microsoft.Playwright
open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions

/// <summary> Represents a page context. </summary>
type PageContext =
    {
        /// <summary> Playwright context. </summary>
        PlaywrightContext: PlaywrightContext
        /// <summary> Logger for this context. </summary>
        Logger: ILogger
        /// <summary> Page represented by this context. </summary>
        Page: IPage
        /// <summary> Uri of the page. </summary>
        Uri: Uri
    }

    /// <summary> Get logger factory. </summary>
    member LoggerFactory: ILoggerFactory

module PageContext =
    val openPage: uri: string -> context: PlaywrightContext -> Result<PageContext, #exn>
    val closePage: context: PageContext -> Result<PlaywrightContext, #exn>
    val setDefaultTimeOut: timeOut: TimeSpan -> context: PageContext -> PageContext