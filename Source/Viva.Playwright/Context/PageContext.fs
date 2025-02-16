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

/// <summary> Extensions for <see cref="PageContext"/>. </summary>
/// <category> Extensions </category>
[<Extension>]
type PageContextExtensions =
    /// <summary> Close the page. </summary>
    /// <param name="context"> The page context. </param>
    /// <returns> If successful, the playwright context; otherwise, the error. </returns>
    [<Extension>]
    static member Close(context: PageContext) =
        result {
            let logger = context.Logger

            do!
                context.Page.CloseAsync().AwaitResult()
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to close {page}.", context.Uri))

            logger.LogInformation("Closed {page}.", context.Uri)
            return context.PlaywrightContext
        }

    /// <summary> Close the page and <see cref="PlaywrightContext"/>. </summary>
    /// <param name="context"> The page context. </param>
    /// <returns> If successful, <c>unit</c>; otherwise, the error. </returns>
    /// <remarks>
    /// This method is a shortcut for calling <see cref="PageContext.Close"/> and <see cref="PlaywrightContext.Close"/> sequentially.
    /// Use this method when the current page is the only page in the context or any page(s) that may be open should be closed.
    ///  </remarks>
    [<Extension>]
    static member CloseAll(context: PageContext) = context.Close() >>= _.Close()

    /// <summary> Open a new page. </summary>
    /// <param name="context"> The playwright context. </param>
    /// <param name="uri"> The uri of the page. </param>
    /// <returns> If successful, the page context; otherwise, the error. </returns>
    [<Extension>]
    static member Open(context: PlaywrightContext, uri: string) =
        result {
            let logger = context.LoggerFactory.CreateLogger<PageContext>()

            let! page =
                context.BrowserContext.NewPageAsync().AwaitResult()
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to create a new page while opening {page}.", uri))

            let! _ =
                page.GotoAsync(uri).AwaitResult()
                |> Result.teeError(fun ex -> logger.LogError(ex, "Failed to open {page}.", uri))

            logger.LogInformation("Opened {page}.", uri)

            return {
                PlaywrightContext = context
                Logger = logger
                Page = page
                Uri = Uri uri
            }
        }

    /// <summary> Set the default timeout for all page actions. </summary>
    /// <param name="context"> The page context. </param>
    /// <param name="timeOut"> The time out. </param>
    /// <returns> The page context. </returns>
    [<Extension>]
    static member SetDefaultTimeOut(context: PageContext, timeOut: TimeSpan) =
        context.Page.SetDefaultTimeout(timeOut.TotalMilliseconds |> float32)
        context

    /// <summary> Get a locator context by label. </summary>
    /// <param name="context"> The page context. </param>
    /// <param name="label"> The label of the element. </param>
    /// <param name="exact"> If true, the <paramref name="label"/> should be an exact match.
    /// This helps in case the <paramref name="label"/> matches itself and also partially other elements. </param>
    /// <returns> The locator context. </returns>
    [<Extension>]
    static member GetByLabel(context: PageContext, label: string, [<Optional>] exact) =
        LocatorContext.Create(context.Page.GetByLabel(label, PageGetByLabelOptions(Exact = exact)), context, $"Label => '%s{label}'")

    /// <summary> Get a locator context by role. </summary>
    /// <param name="context"> The page context. </param>
    /// <param name="role"> The role of the element. </param>
    /// <param name="name"> The name of the element. </param>
    /// <param name="checked"> The checked state of the element. </param>
    /// <param name="disabled"> The disabled state of the element. </param>
    /// <param name="exact"> If true, the <paramref name="name"/> should be an exact match.
    /// This helps in case the <paramref name="name"/> matches itself and also partially other elements. </param>
    /// <param name="expanded"> The expanded state of the element. </param>
    /// <param name="includeHidden"> If true, include hidden elements in the search. </param>
    /// <param name="level"> The level of the element. </param>
    /// <param name="NameRegex"> The regular expression to match the name of the element. </param>
    /// <param name="nameString"> The string to match accessible name of the element. </param>
    /// <param name="pressed"> The pressed state of the element. </param>
    /// <param name="selected"> The selected state of the element. </param>
    /// <returns> The locator context. </returns>
    [<Extension>]
    static member GetByRole
        (
            context: PageContext,
            role,
            [<Optional>] name,
            [<Optional>] ``checked``,
            [<Optional>] disabled,
            [<Optional>] exact,
            [<Optional>] expanded,
            [<Optional>] includeHidden,
            [<Optional>] level,
            [<Optional>] NameRegex,
            [<Optional>] nameString,
            [<Optional>] pressed,
            [<Optional>] selected
        ) =
        LocatorContext.Create(
            context.Page.GetByRole(
                role,
                PageGetByRoleOptions(
                    Name = name,
                    Checked = ``checked``,
                    Disabled = disabled,
                    Exact = exact,
                    Expanded = expanded,
                    IncludeHidden = includeHidden,
                    Level = level,
                    NameRegex = NameRegex,
                    NameString = nameString,
                    Pressed = pressed,
                    Selected = selected
                )
            ),
            context,
            if name = null then
                $"Aria Role Element => '%O{role}'"
            else
                $"Aria Role Element => '%O{role}' named '%s{name}'"
        )

    /// <summary> Get a locator context by placeholder. </summary>
    /// <param name="context"> The page context. </param>
    /// <param name="placeHolder"> The placeholder of the element. </param>
    /// <param name="exact"> If true, the <paramref name="placeHolder"/> should be an exact match.
    /// This helps in case the <paramref name="placeHolder"/> matches itself and also partially other elements. </param>
    /// <returns> The locator context. </returns>
    [<Extension>]
    static member GetByPlaceholder(context: PageContext, placeHolder: string, [<Optional>] exact) =
        LocatorContext.Create(
            context.Page.GetByPlaceholder(placeHolder, PageGetByPlaceholderOptions(Exact = exact)),
            context,
            $"placeholder => '%s{placeHolder}'"
        )

    /// <summary> Get a locator context by selector. </summary>
    /// <param name="context"> The page context. </param>
    /// <param name="selector"> The selector of the element. </param>
    /// <param name="hasText"> Matches with the text in the element. </param>
    /// <param name="hasNotText"> Matches when the text is not in the element. </param>
    /// <param name="hasTextRegex"> Matches with the text in the element using a regular expression. </param>
    /// <param name="hasNotTextRegex"> Matches element when the regular expression does not match. </param>
    /// <returns> The locator context. </returns>
    [<Extension>]
    static member GetBySelector
        (
            context: PageContext,
            selector,
            [<Optional>] hasText,
            [<Optional>] hasNotText,
            [<Optional>] hasTextRegex,
            [<Optional>] hasNotTextRegex
        ) =
        LocatorContext.Create(
            context.Page.Locator(
                selector,
                PageLocatorOptions(
                    HasNotText = hasNotText,
                    HasNotTextRegex = hasNotTextRegex,
                    HasText = hasText,
                    HasTextRegex = hasTextRegex
                )
            ),
            context,
            $"selector => '%s{selector}'"
        )

    /// <summary> Get a locator context by text. </summary>
    /// <param name="context"> The page context. </param>
    /// <param name="text"> The text of the element. </param>
    /// <param name="exact"> If true, the <paramref name="text"/> should be an exact match.
    /// This helps in case the <paramref name="text"/> matches itself and also partially other elements. </param>
    /// <returns> The locator context. </returns>
    [<Extension>]
    static member GetByText(context: PageContext, text: string, [<Optional>] exact) =
        LocatorContext.Create(context.Page.GetByText(text, PageGetByTextOptions(Exact = exact)), context, $"text => '%s{text}'")

    /// <summary> Get a locator context by element title. </summary>
    /// <param name="context"> The page context. </param>
    /// <param name="title"> The title of the element. </param>
    /// <param name="exact"> If true, the <paramref name="title"/> should be an exact match.
    /// This helps in case the <paramref name="title"/> matches itself and also partially other elements. </param>
    /// <returns> The locator context. </returns>
    [<Extension>]
    static member GetByTitle(context: PageContext, title: string, [<Optional>] exact) =
        LocatorContext.Create(context.Page.GetByTitle(title, PageGetByTitleOptions(Exact = exact)), context, $"title => '%s{title}'")