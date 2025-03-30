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

module ButtonTheme =
    open Avalonia.Animation

    let template: IControlTemplate =
        FuncControlTemplate<Button>(fun window namescope ->
            let presenter =
                ContentPresenter(Name = "PART_ContentPresenter", RecognizesAccessKey = true, ClipToBounds = true)
                    .RegisterIn(namescope)
                    .TemplateBinding(ContentPresenter.BorderBrushProperty, Button.BorderBrushProperty)
                    .TemplateBinding(ContentPresenter.BackgroundProperty, Button.BackgroundProperty)
                    .TemplateBinding(ContentPresenter.BorderThicknessProperty, Button.BorderThicknessProperty)
                    .TemplateBinding(ContentPresenter.CornerRadiusProperty, Button.CornerRadiusProperty)
                    .TemplateBinding(ContentPresenter.ContentProperty, Button.ContentProperty)
                    .TemplateBinding(ContentPresenter.ContentTemplateProperty, Button.ContentTemplateProperty)
                    .TemplateBinding(ContentPresenter.HorizontalContentAlignmentProperty, Button.HorizontalContentAlignmentProperty)
                    .TemplateBinding(ContentPresenter.PaddingProperty, Button.PaddingProperty)
                    .TemplateBinding(ContentPresenter.VerticalContentAlignmentProperty, Button.VerticalContentAlignmentProperty)

            presenter
        )

    let create(resources: ResourceDictionary) =
        resources.Add(
            typeof<Button>,
            ControlTheme(typeof<Button>)
                .Set(Button.TemplateProperty, template)
                .DynamicSet(Button.BackgroundProperty, Resource.Control)
                .DynamicSet(Button.ForegroundProperty, Resource.Foreground)
                .Set(
                    Button.TransitionsProperty,
                    Transitions()
                        .AddBrush(Button.BackgroundProperty)
                        .AddBrush(Button.BorderBrushProperty)
                        .AddCornerRadius(Button.CornerRadiusProperty)
                        .AddDouble(Button.FontSizeProperty)
                        .AddDouble(Button.HeightProperty)
                        .AddDouble(Button.OpacityProperty)
                        .AddDouble(Button.WidthProperty)
                )
        )

        resources