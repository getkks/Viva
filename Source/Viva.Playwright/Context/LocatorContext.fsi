namespace Viva.Playwright

open System
open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling
open Microsoft.Playwright
open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions

/// <summary> Represents a locator context. </summary>
type LocatorContext = {
    PageContext: PageContext
    Logger: ILogger
    Locator: ILocator
    LocatorString: string
}

module LocatorContext =
    /// <summary> Ignore the given <c>LocatorContext</c> and return the <c>PageContext</c>. </summary>
    /// <param name="context"> The locator context. </param>
    /// <returns> The page context. </returns>
    val ignoreActiveLocator: context: LocatorContext -> PageContext
    /// <summary> Get locator by label. </summary>
    /// <param name="name"> Name of the label. </param>
    /// <param name="exact"> <c>true</c> if the label should be exact. </param>
    /// <param name="context"> The page context. </param>
    /// <returns> The locator. </returns>
    val getByLabel: name: string -> exact: bool voption -> context: PageContext -> LocatorContext

    /// <summary> Get locator by role. </summary>
    /// <param name="role"> Role of the element. </param>
    /// <param name="name"> Name of the element. </param>
    /// <param name="exact"> <c>true</c> if the locator should be exact. </param>
    /// <param name="context"> The page context. </param>
    /// <returns> The locator. </returns>
    val getByRole: role: AriaRole -> name: string -> exact: bool voption -> context: PageContext -> LocatorContext

    /// <summary> Get locator by selector. </summary>
    /// <param name="selector"> Selector for the element. </param>
    /// <param name="hasText"> Text to match. </param>
    /// <param name="context"> The page context. </param>
    val getBySelector: selector: string -> hasText: string voption -> context: PageContext -> LocatorContext

    /// <summary> Click the element represented by the locator. </summary>
    /// <param name="timeOut"> Custom time out for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="delay"> Custom delay for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="context"> The locator context. </param>
    /// <returns> The locator context. </returns>
    val click: timeOut: TimeSpan voption -> delay: TimeSpan voption -> context: LocatorContext -> Result<LocatorContext, #exn>

    /// <summary> Click the element represented by the given label. </summary>
    /// <param name="name"> Name of the element. </param>
    /// <param name="timeOut"> Custom time out for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="delay"> Custom delay for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="context"> The page context. </param>
    /// <returns> If successful, returns <c>PageContext</c> else error. </returns>
    val clickByLabel:
        name: string -> timeOut: voption<TimeSpan> -> delay: voption<TimeSpan> -> context: PageContext -> Result<PageContext, #exn>

    /// <summary> Click the element represented by the given role with the given name. </summary>
    /// <param name="role"> Role of the element. </param>
    /// <param name="name"> Name of the element. </param>
    /// <param name="timeOut"> Custom time out for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="delay"> Custom delay for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="context"> The page context. </param>
    /// <returns> If successful, returns <c>PageContext</c> else error. </returns>
    val clickByRole:
        role: AriaRole ->
        name: string ->
        timeOut: TimeSpan voption ->
        delay: TimeSpan voption ->
        context: PageContext ->
            Result<PageContext, #exn>

    /// <summary> Click the element represented by the given selector. </summary>
    /// <param name="selector"> Selector for the element. </param>
    /// <param name="hasText"> Text to match. </param>
    /// <param name="timeOut"> Custom time out for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="delay"> Custom delay for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="context"> The page context. </param>
    /// <returns> If successful, returns <c>PageContext</c> else error. </returns>
    val clickBySelector:
        selector: string ->
        hasText: voption<string> ->
        timeOut: voption<TimeSpan> ->
        delay: voption<TimeSpan> ->
        context: PageContext ->
            Result<PageContext, #exn>

    /// <summary> Fill the element represented by the locator. </summary>
    /// <param name="value"> The value to fill. </param>
    /// <param name="timeOut"> Custom time out for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="context"> The locator context. </param>
    /// <returns> The locator context. </returns>
    val fill: value: string -> timeOut: TimeSpan voption -> context: LocatorContext -> Result<LocatorContext, #exn>

    /// <summary> Send key stroke(s) to the element represented by the locator. </summary>
    /// <param name="value"> The key stroke(s) to send. </param>
    /// <param name="delay"> Custom delay for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="timeOut"> Custom time out for this action. <c>ValueOption.None</c> for default. </param>
    /// <param name="context"> The locator context. </param>
    /// <returns> The locator context. </returns>
    val press:
        value: string -> timeOut: TimeSpan voption -> delay: TimeSpan voption -> context: LocatorContext -> Result<LocatorContext, #exn>