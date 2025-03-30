namespace Viva.UI.Controls.Host

open System
open System.Runtime.CompilerServices

open Avalonia
open Avalonia.Controls
open Avalonia.Styling

open Material.Icons.Avalonia

open Avalonia.Controls
open Avalonia.Controls.Presenters
open Avalonia.Controls.Primitives
open Avalonia.Layout
open Avalonia.Media
open Avalonia.Platform

open Elmish
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Hosts
open Avalonia.FuncUI.Types

open Wacton.Unicolour

open Material.Icons
open Material.Icons.Avalonia

open FsToolkit.ErrorHandling

open Viva.UI
// open Viva.UI.Styles.Builders
// open Viva.UI.Context
open Viva.UI.Controls
open Viva.UI.Runtime
// open Viva.UI.Styles

#nowarn 3261

type Window() =
    inherit HostWindow()

    do
        base.Title <- "Viva"
        base.CanResize <- true
        base.Width <- 1980
        base.Height <- 1080

#if DEBUG
        base.AttachDevTools()
#endif
    override this.OnApplyTemplate(e: Primitives.TemplateAppliedEventArgs) : unit =
        base.OnApplyTemplate(e: Primitives.TemplateAppliedEventArgs)

    override this.OnPointerReleased(e: Input.PointerReleasedEventArgs) : unit =
        base.OnPointerReleased(e: Input.PointerReleasedEventArgs)
        if this.ActualThemeVariant = ThemeVariant.Dark then
            this.RequestedThemeVariant <- ThemeVariant.Light
        else
            this.RequestedThemeVariant <- ThemeVariant.Dark

    override this.OnPointerPressed(e: Input.PointerPressedEventArgs) : unit =
        base.OnPointerPressed(e: Input.PointerPressedEventArgs)
        this.BeginMoveDrag e

type Page =
    | Home
    | Schedule
    | Settings

    override this.ToString() =
        match this with
        | Home -> nameof Home
        | Schedule -> nameof Schedule
        | Settings -> nameof Settings

    member this.Icon =
        match this with
        | Home -> MaterialIconKind.Home
        | Schedule -> MaterialIconKind.Schedule
        | Settings -> MaterialIconKind.Settings

type PageDetails = {
    Page: Page
    IsActive: bool
} with

    member this.Icon = this.Page.Icon

    [<Extension>]
    static member UpdateActivePage(values, activePage) =
        values |> Array.map(fun x -> { x with IsActive = x.Page = activePage })

module Window =
    let appCurrentTheme() =
        match Application.Current with
        | null -> ThemeVariant.Default
        | application -> application.ActualThemeVariant

    type WindowStatus =
        | Loading
        | Loaded
        | Closing
        | Closed

    type State = {
        ActivePage: Page
        SidePaneOpen: bool
        ThemeVariant: ThemeVariant
        Window: Window
        WindowStatus: WindowStatus
        Pages: PageDetails[]
    }

    type Message =
        | ThemeChanged of ThemeVariant
        | WindowStatusChanged of WindowStatus
        | ActivePageChanged of Page
        | SidePaneStatusChanged of bool

    let init(window: Window) =
        {
            ActivePage = Home
            SidePaneOpen = true
            ThemeVariant = appCurrentTheme()
            Window = window
            WindowStatus = Loading
            Pages = [|
                {
                    Page = Home
                    IsActive = true
                }
                {
                    Page = Schedule
                    IsActive = false
                }
                {
                    Page = Settings
                    IsActive = false
                }
            |]
        },
        Cmd.none

    let update msg state =
        match msg with
        | ThemeChanged theme ->
            match Application.Current with
            | null -> ()
            | application -> application.RequestedThemeVariant <- theme

            { state with ThemeVariant = theme }, Cmd.none
        | WindowStatusChanged status ->
            { state with WindowStatus = status },
            match Application.Current with
            | null -> Cmd.none
            | application ->
                if state.ThemeVariant <> application.ActualThemeVariant then
                    state.ThemeVariant |> ThemeChanged |> Cmd.ofMsg
                else
                    Cmd.none
        | ActivePageChanged selection ->
            {
                state with
                    ActivePage = selection
                    Pages = state.Pages.UpdateActivePage selection
            },
            Cmd.none
        | SidePaneStatusChanged status -> { state with SidePaneOpen = status }, Cmd.none

    let view state dispatch =
        if state.WindowStatus = Loading then
            let window = state.Window
            window.Closing.Add(fun _ -> Closing |> WindowStatusChanged |> dispatch)
            window.Closed.Add(fun _ -> Closed |> WindowStatusChanged |> dispatch)
            window.Loaded.Add(fun _ -> Loaded |> WindowStatusChanged |> dispatch)

        SplitView.create [
            SplitView.isPaneOpen state.SidePaneOpen
            SplitView.openPaneLength 300
            SplitView.compactPaneLengthProperty 64
            SplitView.displayMode SplitViewDisplayMode.CompactInline
            SplitView.pane(
                ExperimentalAcrylicBorder.create [
                    ExperimentalAcrylicBorder.material(
                        ExperimentalAcrylicMaterial(
                            BackgroundSource = AcrylicBackgroundSource.Digger,
                            MaterialOpacity = 0.5,
                            TintColor = Unicolour(ColourSpace.Okhsl, 30, 100, 0.10).ToColor(),
                            TintOpacity = 1
                        )
                    )
                    ExperimentalAcrylicBorder.child(
                        StackPanel.create [
                            StackPanel.orientation Orientation.Vertical
                            StackPanel.children [
                                Button.create [
                                    Button.classes [
                                        "Icon"
                                        "Accent"
                                    ]
                                    Button.borderThickness 0
                                    if state.SidePaneOpen then
                                        Button.horizontalAlignment HorizontalAlignment.Right
                                    else
                                        Button.horizontalAlignment HorizontalAlignment.Center
                                    Button.margin(0, 20)
                                    if state.SidePaneOpen then
                                        Button.onClick(fun _ -> false |> SidePaneStatusChanged |> dispatch)
                                    else
                                        Button.onClick(fun _ -> true |> SidePaneStatusChanged |> dispatch)
                                    Button.content(
                                        MaterialIcon.create [
                                            MaterialIcon.width 32
                                            MaterialIcon.height 32
                                            MaterialIcon.kind(
                                                if state.SidePaneOpen then
                                                    MaterialIconKind.MenuOpen
                                                else
                                                    MaterialIconKind.MenuClose
                                            )
                                        ]
                                    )
                                ]
                                for pageDetails in state.Pages do
                                    MenuItem.create [
                                        MenuItem.padding(0, 10)
                                        MenuItem.margin(0, 10)
                                        if state.SidePaneOpen then
                                            MenuItem.header pageDetails.Page
                                        if pageDetails.Page <> state.ActivePage then
                                            MenuItem.onClick(fun _ -> pageDetails.Page |> ActivePageChanged |> dispatch)
                                        MenuItem.isSelected pageDetails.IsActive
                                        MenuItem.icon(
                                            MaterialIcon.create [
                                                MaterialIcon.verticalAlignment VerticalAlignment.Center
                                                MaterialIcon.width 32
                                                MaterialIcon.height 32
                                                MaterialIcon.kind pageDetails.Icon
                                                MaterialIcon.horizontalAlignment HorizontalAlignment.Center
                                            ]
                                        )
                                    ]
                                Separator.create []
                                ListBox.create [
                                    ListBox.classes [ "Stack" ]
                                    ListBox.onSelectedIndexChanged(fun index -> state.Pages[index].Page |> ActivePageChanged |> dispatch)
                                    ListBox.viewItems [
                                        for pageDetails in state.Pages do
                                            ListBoxItem.create [
                                                ListBoxItem.padding(0, 10)
                                                ListBoxItem.margin(0, 10)
                                                ListBoxItem.content(
                                                    if state.SidePaneOpen then
                                                        Grid.create [
                                                            Grid.columnDefinitions "64,*"
                                                            Grid.children [
                                                                MaterialIcon.create [
                                                                    MaterialIcon.verticalAlignment VerticalAlignment.Center
                                                                    MaterialIcon.width 32
                                                                    MaterialIcon.height 32
                                                                    MaterialIcon.kind pageDetails.Icon
                                                                    MaterialIcon.horizontalAlignment HorizontalAlignment.Center
                                                                ]
                                                                TextBlock.create [
                                                                    Grid.column 1
                                                                    pageDetails.Page.ToString() |> TextBlock.text
                                                                    TextBlock.fontSize 14.
                                                                    TextBlock.verticalAlignment VerticalAlignment.Center
                                                                ]
                                                            ]
                                                        ]
                                                        :> IView
                                                    else
                                                        MaterialIcon.create [
                                                            MaterialIcon.verticalAlignment VerticalAlignment.Center
                                                            MaterialIcon.width 32
                                                            MaterialIcon.height 32
                                                            MaterialIcon.kind pageDetails.Icon
                                                            MaterialIcon.horizontalAlignment HorizontalAlignment.Center
                                                        ]
                                                )
                                            ]
                                    ]
                                ]

                                Button.create [
                                    Button.classes [
                                        "Icon"
                                        "Accent"
                                    ]
                                    Button.verticalAlignment VerticalAlignment.Bottom
                                    Button.borderThickness 0
                                    Button.horizontalAlignment HorizontalAlignment.Center
                                    Button.margin(0, 20)
                                    if appCurrentTheme() = ThemeVariant.Dark then
                                        Button.onClick(fun _ -> ThemeVariant.Light |> ThemeChanged |> dispatch)
                                    else
                                        Button.onClick(fun _ -> ThemeVariant.Dark |> ThemeChanged |> dispatch)
                                    Button.content(
                                        MaterialIcon.create [
                                            MaterialIcon.width 32
                                            MaterialIcon.height 32
                                            MaterialIcon.kind MaterialIconKind.ThemeLightDark
                                        ]
                                    )
                                ]
                            ]
                        ]
                    )
                ]
            )
            SplitView.content(
                Panel.create [
                    Panel.children [
                        TextBlock.create [
                            // state.ActivePage.ToString() |> TextBlock.text
                            "The quick brown fox jumps over the lazy dog" |> TextBlock.text
                            TextBlock.fontSize 72.
                            TextBlock.horizontalAlignment HorizontalAlignment.Center
                            TextBlock.verticalAlignment VerticalAlignment.Center
                        ]
                        CaptionButtons.create []
                    ]
                ]
            )
        ]