namespace Viva.UI.Runtime

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices

open Avalonia
open Avalonia.Animation
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.Data
open Avalonia.Data.Core
open Avalonia.Markup.Xaml.MarkupExtensions
open Avalonia.Markup.Xaml.XamlIl.Runtime
open Avalonia.Media
open Avalonia.Styling

open FsToolkit.ErrorHandling
open Wacton.Unicolour

type TransitionsExtensions() =
    static member val DefaultEasing: Easings.Easing = Easings.CubicEaseOut() with get, set
    static member val DefaultDuration: TimeSpan = TimeSpan.FromMilliseconds 750 with get, set

    [<Extension>]
    static member Add<'TTransition, 'TProp when 'TTransition :> TransitionBase and 'TTransition: (new: unit -> 'TTransition)>
        (
            this: Transitions,
            property: 'TProp AvaloniaProperty,
            [<Optional>] duration: TimeSpan voption,
            [<Optional>] easing: Easings.Easing voption
        ) =
        let transition = new 'TTransition()
        transition.Property <- property
        transition.Duration <- duration |> ValueOption.defaultValue TransitionsExtensions.DefaultDuration
        transition.Easing <- easing |> ValueOption.defaultValue TransitionsExtensions.DefaultEasing
        this.Add transition
        this

    [<Extension>]
    static member AddBoxShadow(this: Transitions, property: _ AvaloniaProperty, [<Optional>] duration, [<Optional>] easing) =
        this.Add<BoxShadowsTransition, BoxShadows>(property, duration, easing)

    [<Extension>]
    static member AddBrush(this: Transitions, property: _ AvaloniaProperty, [<Optional>] duration, [<Optional>] easing) =
        this.Add<BrushTransition, IBrush | null>(property, duration, easing)

    [<Extension>]
    static member AddColor(this: Transitions, property: _ AvaloniaProperty, [<Optional>] duration, [<Optional>] easing) =
        this.Add<ColorTransition, Color>(property, duration, easing)

    [<Extension>]
    static member AddCornerRadius(this: Transitions, property: _ AvaloniaProperty, [<Optional>] duration, [<Optional>] easing) =
        this.Add<CornerRadiusTransition, CornerRadius>(property, duration, easing)

    [<Extension>]
    static member AddDouble(this: Transitions, property: _ AvaloniaProperty, [<Optional>] duration, [<Optional>] easing) =
        this.Add<DoubleTransition, double>(property, duration, easing)

    [<Extension>]
    static member AddPoint(this: Transitions, property: _ AvaloniaProperty, [<Optional>] duration, [<Optional>] easing) =
        this.Add<PointTransition, Point>(property, duration, easing)

    [<Extension>]
    static member AddSize(this: Transitions, property: _ AvaloniaProperty, [<Optional>] duration, [<Optional>] easing) =
        this.Add<SizeTransition, Size>(property, duration, easing)

    [<Extension>]
    static member AddThickness(this: Transitions, property: _ AvaloniaProperty, [<Optional>] duration, [<Optional>] easing) =
        this.Add<ThicknessTransition, Thickness>(property, duration, easing)