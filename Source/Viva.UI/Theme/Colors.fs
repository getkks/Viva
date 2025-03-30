namespace Viva.UI.Theme

open System

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.Controls.Templates
open Avalonia.Controls.Presenters
open Avalonia.Media
open Avalonia.Platform
open Avalonia.Styling
open Avalonia.Markup.Xaml.MarkupExtensions

open Wacton.Unicolour
open Viva.UI.Runtime

module Colors =
    let hue = Random.Shared.Next(0, 360)

    let lightColors = [|
        Resource.Background, Unicolour(ColourSpace.Okhsl, hue, 100, 0.97).ToColor()
        Resource.Control, Unicolour(ColourSpace.Okhsl, hue, 100, 0.50).ToColor()
        Resource.Foreground, Unicolour(ColourSpace.Okhsl, hue, 100, 0.0).ToColor()
    |]

    let darkColors = [|
        Resource.Background, Unicolour(ColourSpace.Okhsl, hue, 100, 0.10).ToColor()
        Resource.Control, Unicolour(ColourSpace.Okhsl, hue, 100, 0.50).ToColor()
        Resource.Foreground, Unicolour(ColourSpace.Okhsl, hue, 0, 1.).ToColor()
    |]

    let addVariantColors (variant: ThemeVariant) (colors: (Resource * Color)[]) (resources: ResourceDictionary) =
        resources.ThemeDictionaries.Add(
            variant,
            colors
            |> Array.fold
                (fun (resource: ResourceDictionary) (key, value) ->
                    resource.Add(key.AsBrush.Name, SolidColorBrush value)
                    resource.Add(key.AsColor.Name, value)
                    resource
                )
                (ResourceDictionary())
        )

        resources

    let create(resources: ResourceDictionary) =
        resources
        |> addVariantColors ThemeVariant.Light lightColors
        |> addVariantColors ThemeVariant.Dark darkColors