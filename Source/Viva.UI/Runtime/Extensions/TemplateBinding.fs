namespace Viva.UI.Runtime

open System.Runtime.CompilerServices

open Avalonia
open Avalonia.Data

[<Extension>]
type TemplateBindingExtensions =
    [<Extension>]
    static member TemplateBinding(this: #AvaloniaObject, property: AvaloniaProperty, sourceProperty: AvaloniaProperty) =
        this.Bind(property, TemplateBinding(sourceProperty).ProvideValue()) |> ignore
        this