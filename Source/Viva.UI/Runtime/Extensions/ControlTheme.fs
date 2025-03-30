namespace Viva.UI.Runtime

open System.Runtime.CompilerServices

open Avalonia
open Avalonia.Markup.Xaml.MarkupExtensions
open Avalonia.Media
open Avalonia.Styling

open Wacton.Unicolour

open Viva.UI.Runtime

type ControlThemeExtensions =
    [<Extension>]
    static member DynamicSet(theme: ControlTheme, property: 'TProp AvaloniaProperty, resourceName: #IResource<'TProp>) =
        theme.DynamicSet(property, resourceName.Name)

    [<Extension>]
    static member DynamicSet(theme: ControlTheme, property: 'TProp AvaloniaProperty, resourceName: string) =
        Setter(Property = property, Value = DynamicResourceExtension resourceName)
        |> theme.Add

        theme

    [<Extension>]
    static member Set(theme: ControlTheme, property: 'TProp AvaloniaProperty, value: 'TProp) =
        Setter(Property = property, Value = value) |> theme.Add
        theme

    [<Extension>]
    static member Set(theme: ControlTheme, property: IBrush AvaloniaProperty, value: Unicolour) = theme.Set(property, value.ToBrush())

    [<Extension>]
    static member Set(theme: ControlTheme, property: Color AvaloniaProperty, value: Unicolour) = theme.Set(property, value.ToColor())

    [<Extension>]
    static member StaticSet(theme: ControlTheme, property: 'TProp AvaloniaProperty, resourceName: #IResource<'TProp>) =
        theme.StaticSet(property, resourceName.Name)

    [<Extension>]
    static member StaticSet(theme: ControlTheme, property: 'TProp AvaloniaProperty, resourceName: string) =
        Setter(Property = property, Value = StaticResourceExtension resourceName)
        |> theme.Add

        theme

    [<Extension>]
    static member inline StaticSet(theme: ControlTheme, [<InlineIfLambda>] selector: Selector -> Selector, builder: Style -> Style) =
        let style = Style()
        style.Selector <- Unchecked.defaultof<Selector>.Nesting() |> selector
        style |> builder |> theme.Add
        theme