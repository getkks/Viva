namespace Viva.UI.Theme

open System
open System.Runtime.CompilerServices

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Templates
open Avalonia.Controls.Presenters
open Avalonia.Controls.Primitives
open Avalonia.Media
open Avalonia.Platform
open Avalonia.Styling
open Avalonia.Markup.Xaml.MarkupExtensions

open Wacton.Unicolour

open Viva.UI
open Viva.UI.Runtime
open Viva.UI.Runtime.Extensions

module WindowTheme =
    open Avalonia.Animation

    let template: IControlTemplate =
        FuncControlTemplate<Window>(fun window namescope ->
            let panel = Panel()

            Border(Name = "PART_Border", IsHitTestVisible = false).RegisterIn namescope
            |> panel.Children.Add

            Border(IsHitTestVisible = false).TemplateBinding(Border.BackgroundProperty, Window.BackgroundProperty)
            |> panel.Children.Add

            Panel(Background = Brushes.Transparent).TemplateBinding(Border.MarginProperty, Window.SystemDecorationsProperty)
            |> panel.Children.Add

            let visualLayer = VisualLayerManager()
            panel.Children.Add visualLayer

            let presenter =
                ContentPresenter(Name = "PART_ContentPresenter")
                    .RegisterIn(namescope)
                    .TemplateBinding(ContentPresenter.ContentTemplateProperty, Window.ContentTemplateProperty)
                    .TemplateBinding(ContentPresenter.ContentProperty, Window.ContentProperty)
                    .TemplateBinding(ContentPresenter.MarginProperty, Window.PaddingProperty)
                    .TemplateBinding(ContentPresenter.VerticalContentAlignmentProperty, Window.VerticalContentAlignmentProperty)
                    .TemplateBinding(ContentPresenter.HorizontalContentAlignmentProperty, Window.HorizontalContentAlignmentProperty)

            presenter.Background <- Brushes.Transparent
            visualLayer.Child <- presenter

            panel
        )

    let create(resources: ResourceDictionary) =
        resources.Add(
            typeof<Window>,
            ControlTheme(typeof<Window>)
                .Set(Window.TemplateProperty, template)
                .Set(Window.MarginProperty, Thickness 0)
                .Set(Window.TransparencyLevelHintProperty, [ WindowTransparencyLevel.AcrylicBlur ])
                .Set(Window.ExtendClientAreaChromeHintsProperty, ExtendClientAreaChromeHints.NoChrome)
                .Set(Window.ExtendClientAreaTitleBarHeightHintProperty, -1)
                .Set(Window.ExtendClientAreaToDecorationsHintProperty, true)
                .Set(
                    Window.SystemDecorationsProperty,
                    (if OperatingSystem.IsLinux() then
                         SystemDecorations.None
                     else
                         SystemDecorations.Full)
                )
                .DynamicSet(Window.FontFamilyProperty, "Poppins")
                .Set(Window.FontSizeProperty, 14.)
                .DynamicSet(Window.BackgroundProperty, Resource.Background)
                .DynamicSet(Window.ForegroundProperty, Resource.Foreground)
        )

        resources