namespace Viva.Playwright

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling
open Microsoft.Playwright
open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions

/// <summary> Extensions for <see cref="LocatorContext"/>. </summary>
/// <category> Extensions </category>
[<Extension>]
type LocatorContextExtensions =

    /// <summary> Closes the <see cref="LocatorContext"/>. </summary>
    /// <param name="context"> The <see cref="LocatorContext"/>. </param>
    /// <returns> The <see cref="PageContext"/>. </returns>
    [<Extension>]
    static member Close(context: LocatorContext) = context.PageContext

    [<Extension>]
    static member Refine(context: LocatorContext, refine) =
        let newLocator, newLocatorString = refine context.Locator context.LocatorString
        LocatorContext.Create(newLocator, context.PageContext, newLocatorString)

    /// <summary> Click the element represented by the locator. </summary>
    /// <param name="this"> The <see cref="LocatorContext"/>. </param>
    /// <param name="button"> The mouse button to click. Defaults to <see cref="MouseButton.Left"/>. </param>
    /// <param name="clickCount"> The number of times to click. Defaults to <c>1</c>. </param>
    /// <param name="delay"> The time to wait before clicking. Defaults to <c>0 ms</c>. </param>
    /// <param name="force"> <c>true</c> to bypass the actionability checks. Defaults to <c>false</c>. </param>
    /// <param name="modifiers"> Modifier keys to press. Any keys pressed will be passed if <paramref name="force"/> is not set. Defaults to <c>null</c>.</param>
    /// <param name="position"> The relative position inside the element to click. </param>
    /// <param name="timeout"> The maximum time to wait for the element to be clickable. Defaults to <c>0 ms</c>. </param>
    /// <param name="Trial"> Performs actionability checks including key presses but mouse click is not performed. Defaults to <c>false</c>. </param>
    /// <returns> If successful, the <see cref="LocatorContext"/>; otherwise, the error. </returns>
    [<Extension>]
    static member Click
        (
            this: LocatorContext,
            [<Optional; DefaultParameterValue(MouseButton.Left)>] button,
            [<Optional; DefaultParameterValue(1)>] clickCount,
            [<Optional>] delay: TimeSpan,
            [<Optional; DefaultParameterValue(false)>] force,
            [<Optional>] modifiers,
            [<Optional; DefaultParameterValue(null: Position | null)>] position,
            [<Optional>] timeout: TimeSpan,
            [<Optional; DefaultParameterValue(false)>] Trial
        ) =
        result {
            let logger = this.Logger

            do!
                this.Locator
                    .ClickAsync(
                        LocatorClickOptions(
                            Button = Nullable button,
                            ClickCount = Nullable clickCount,
                            Delay = delay.ToNullableMilliseconds(),
                            Force = Nullable force,
                            Modifiers = modifiers,
                            Position = position,
                            Timeout = timeout.ToNullableMilliseconds(),
                            Trial = Nullable Trial
                        )
                    )
                    .AwaitResult()
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to click {locator}.", this.LocatorString))

            logger.LogInformation("Clicked {locator}.", this.LocatorString)
            return this
        }

    /// <summary> Click the element represented by the locator and close the <see cref="LocatorContext"/>. </summary>
    /// <param name="this"> The <see cref="LocatorContext"/>. </param>
    /// <param name="button"> The mouse button to click. Defaults to <see cref="MouseButton.Left"/>. </param>
    /// <param name="clickCount"> The number of times to click. Defaults to <c>1</c>. </param>
    /// <param name="delay"> The time to wait before clicking. Defaults to <c>0 ms</c>. </param>
    /// <param name="force"> <c>true</c> to bypass the actionability checks. Defaults to <c>false</c>. </param>
    /// <param name="modifiers"> Modifier keys to press. Any keys pressed will be passed if <paramref name="force"/> is not set. Defaults to <c>null</c>.</param>
    /// <param name="position"> The relative position inside the element to click. </param>
    /// <param name="timeout"> The maximum time to wait for the element to be clickable. Defaults to <c>0 ms</c>. </param>
    /// <param name="Trial"> Performs actionability checks including key presses but mouse click is not performed. Defaults to <c>false</c>. </param>
    /// <returns> If successful, the <see cref="PageContext"/>; otherwise, the error. </returns>
    [<Extension>]
    static member ClickAndClose
        (
            this: LocatorContext,
            [<Optional; DefaultParameterValue(MouseButton.Left)>] button,
            [<Optional; DefaultParameterValue(1)>] clickCount,
            [<Optional>] delay: TimeSpan,
            [<Optional; DefaultParameterValue(false)>] force,
            [<Optional>] modifiers,
            [<Optional; DefaultParameterValue(null: Position | null)>] position,
            [<Optional>] timeout: TimeSpan,
            [<Optional; DefaultParameterValue(false)>] Trial
        ) =
        this.Click(button, clickCount, delay, force, modifiers, position, timeout, Trial)
        <!> _.Close()

    /// <summary> Fill the element represented by the locator. </summary>
    /// <param name="this"> The <see cref="LocatorContext"/>. </param>
    /// <param name="text"> The text to fill. </param>
    /// <param name="force"> <c>true</c> to bypass the actionability checks. Defaults to <c>false</c>. </param>
    /// <param name="timeout"> The maximum time to wait for the element to be fillable. Defaults to <c>0 ms</c>. </param>
    /// <returns> If successful, the <see cref="LocatorContext"/>; otherwise, the error. </returns>
    [<Extension>]
    static member Fill
        (this: LocatorContext, text: string, [<Optional; DefaultParameterValue(false)>] force, [<Optional>] timeout: TimeSpan)
        =
        result {
            let logger = this.Logger

            do!
                this.Locator
                    .FillAsync(text, LocatorFillOptions(Force = Nullable force, Timeout = timeout.ToNullableMilliseconds()))
                    .AwaitResult()
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to fill {locator}.", this.LocatorString))

            logger.LogInformation("Filled {locator} with value '{value}'.", this.LocatorString, text)
            return this
        }

    /// <summary> Fill the element represented by the locator and close the <see cref="LocatorContext"/>. </summary>
    /// <param name="this"> The <see cref="LocatorContext"/>. </param>
    /// <param name="text"> The text to fill. </param>
    /// <param name="force"> <c>true</c> to bypass the actionability checks. Defaults to <c>false</c>. </param>
    /// <param name="timeout"> The maximum time to wait for the element to be fillable. Defaults to <c>0 ms</c>. </param>
    /// <returns> If successful, the <see cref="PageContext"/>; otherwise, the error. </returns>
    [<Extension>]
    static member FillAndClose
        (this: LocatorContext, text: string, [<Optional; DefaultParameterValue(false)>] force, [<Optional>] timeout: TimeSpan)
        =
        this.Fill(text, force, timeout) <!> _.Close()

    /// <summary> Press the key(s) to the element represented by the locator. </summary>
    /// <param name="this"> The <see cref="LocatorContext"/>. </param>
    /// <param name="text"> The key stroke(s) to press. </param>
    /// <param name="delay"> The time to wait between key presses. Defaults to <c>0 ms</c>. </param>
    /// <param name="timeout"> The maximum time to wait for the element to be fillable. Defaults to <c>0 ms</c>. </param>
    /// <returns> If successful, the <see cref="LocatorContext"/>; otherwise, the error. </returns>
    [<Extension>]
    static member Press(this: LocatorContext, text: string, [<Optional>] delay: TimeSpan, [<Optional>] timeout: TimeSpan) =
        result {
            let logger = this.Logger

            do!
                this.Locator
                    .PressAsync(
                        text,
                        LocatorPressOptions(Delay = delay.ToNullableMilliseconds(), Timeout = timeout.ToNullableMilliseconds())
                    )
                    .AwaitResult()
                |> Result.teeError(fun ex ->
                    logger.LogError(ex, "Failed to send key stroke(s) '{value}' to {locator}.", text, this.LocatorString)
                )

            logger.LogInformation("Sent key stroke(s) '{value}' to {locator}.", text, this.LocatorString)
            return this
        }

    /// <summary> Press the key(s) to the element represented by the locator and close the <see cref="LocatorContext"/>. </summary>
    /// <param name="this"> The <see cref="LocatorContext"/>. </param>
    /// <param name="text"> The key stroke(s) to press. </param>
    /// <param name="delay"> The time to wait between key presses. Defaults to <c>0 ms</c>. </param>
    /// <param name="timeout"> The maximum time to wait for the element to be fillable. Defaults to <c>0 ms</c>. </param>
    /// <returns> If successful, the <see cref="PageContext"/>; otherwise, the error. </returns>
    [<Extension>]
    static member PressAndClose(this: LocatorContext, text: string, [<Optional>] delay: TimeSpan, [<Optional>] timeout: TimeSpan) =
        this.Press(text, delay, timeout) <!> _.Close()