namespace Viva.Playwright

open System
open Microsoft.Extensions.Logging
open FsToolkit.ErrorHandling
open Microsoft.Playwright
open Viva.Runtime.Helpers
open Viva.Runtime.Helpers.Operator.Result
open Viva.Runtime.Extensions

type LocatorContext = {
    PageContext: PageContext
    Logger: ILogger
    Locator: ILocator
    LocatorString: string
}

module LocatorContext =
    let private toLocatorContext locatorSting (context: PageContext) locator = {
        PageContext = context
        Logger = context.LoggerFactory.CreateLogger<LocatorContext>()
        Locator = locator
        LocatorString = locatorSting
    }

    let ignoreActiveLocator(context: LocatorContext) = context.PageContext

    let getByLabel (name: string) exact (context: PageContext) =
        context.Page.GetByLabel(name, PageGetByLabelOptions(Exact = (exact |> ValueOption.toNullable)))
        |> toLocatorContext $"Label => '{name}'" context

    let getByRole role name exact (context: PageContext) =
        context.Page.GetByRole(role, PageGetByRoleOptions(Name = name, Exact = (exact |> ValueOption.toNullable)))
        |> toLocatorContext $"'{role}' named '{name}'" context

    let getBySelector selector (hasText: string voption) (context: PageContext) =
        context.Page.Locator(
            selector,
            if hasText.IsSome then
                PageLocatorOptions(HasText = hasText.Value)
            else
                null
        )
        |> toLocatorContext $"selector => '{selector}'" context

    let click (timeOut: TimeSpan voption) (delay: TimeSpan voption) (context: LocatorContext) =
        let logger = context.Logger

        context.Locator
            .ClickAsync(LocatorClickOptions(Timeout = timeOut.ToNullableMilliseconds(), Delay = delay.ToNullableMilliseconds()))
            .AwaitResult()
        <!> (fun _ -> context)
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to click {locator}.", context.LocatorString))
        |> Result.tee(fun _ -> logger.LogInformation("Clicked {locator}.", context.LocatorString))

    let clickByLabel name (timeOut: TimeSpan voption) (delay: TimeSpan voption) (context: PageContext) =
        getByLabel name ValueOption.None context |> click timeOut delay
        <!> (fun _ -> context)

    let clickByRole role name (timeOut: TimeSpan voption) (delay: TimeSpan voption) (context: PageContext) =
        getByRole role name ValueOption.None context |> click timeOut delay
        <!> (fun _ -> context)

    let clickBySelector selector (hasText: string voption) (timeOut: TimeSpan voption) (delay: TimeSpan voption) (context: PageContext) =
        getBySelector selector hasText context |> click timeOut delay
        <!> (fun _ -> context)

    let fill value (timeOut: TimeSpan voption) (context: LocatorContext) =
        let logger = context.Logger

        context.Locator.FillAsync(value, LocatorFillOptions(Timeout = timeOut.ToNullableMilliseconds())).AwaitResult()
        <!> (fun _ -> context)
        |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to fill {locator} with value '{value}'.", context.LocatorString, value))
        |> Result.tee(fun _ -> logger.LogInformation("Filled {locator} with value '{value}'.", context.LocatorString, value))

    let press value (timeOut: TimeSpan voption) (delay: TimeSpan voption) (context: LocatorContext) =
        let logger = context.Logger

        context.Locator
            .PressAsync(value, LocatorPressOptions(Timeout = timeOut.ToNullableMilliseconds(), Delay = delay.ToNullableMilliseconds()))
            .AwaitResult()
        <!> (fun _ -> context)
        |> Result.teeError(fun ex ->
            logger.LogError(ex, "Failed to send key stroke(s) '{value}' to {locator}.", value, context.LocatorString)
        )
        |> Result.tee(fun _ -> logger.LogInformation("Sent key stroke(s) '{value}' to {locator}.", value, context.LocatorString))