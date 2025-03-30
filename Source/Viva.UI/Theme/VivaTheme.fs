namespace Viva.UI.Theme

open System

open Avalonia
open Avalonia.Animation
open Avalonia.Controls
open Avalonia.Controls.Documents
open Avalonia.Styling

open Material.Icons.Avalonia

open Viva.UI
open Viva.UI.Runtime
open Viva.UI.Runtime.Extensions

module VivaTheme =

    let create(styleHost: IStyleHost, resources: ResourceDictionary) =
        styleHost.Styles.Add(
            let style = Style()
            style.Selector <- Unchecked.defaultof<Selector>.OfType<Border>()

            style.Add(
                Setter(
                    Property = Border.TransitionsProperty,
                    Value =
                        Transitions()
                            .AddBoxShadow(Border.BoxShadowProperty)
                            .AddBrush(Border.BackgroundProperty)
                            .AddBrush(Border.BorderBrushProperty)
                            .AddCornerRadius(Border.CornerRadiusProperty)
                            .AddDouble(Border.HeightProperty)
                            .AddDouble(Border.OpacityProperty)
                            .AddDouble(Border.WidthProperty)
                )
            )

            style
        )

        styleHost.Styles.Add(
            let style = Style()
            style.Selector <- Unchecked.defaultof<Selector>.OfType<TextBlock>()

            style.Add(
                Setter(
                    Property = TextBlock.TransitionsProperty,
                    Value =
                        Transitions()
                            .AddBrush(TextBlock.ForegroundProperty)
                            .AddBrush(TextBlock.BackgroundProperty)
                            .AddDouble(TextBlock.HeightProperty)
                            .AddDouble(TextBlock.OpacityProperty)
                            .AddDouble(TextBlock.WidthProperty)
                            .AddDouble(TextBlock.FontSizeProperty)
                )
            )

            style
        )

        styleHost.Styles.Add(
            let style = Style()
            style.Selector <- Unchecked.defaultof<Selector>.OfType<TextElement>()

            style.Add(
                Setter(
                    Property = TextBlock.TransitionsProperty,
                    Value =
                        Transitions()
                            .AddBrush(TextElement.ForegroundProperty)
                            .AddBrush(TextElement.BackgroundProperty)
                            .AddDouble(TextElement.FontSizeProperty)
                )
            )

            style
        )

        styleHost.Styles.Add(
            let style = Style()
            style.Selector <- Unchecked.defaultof<Selector>.OfType<MaterialIcon>()

            style.Add(
                Setter(
                    Property = TextBlock.TransitionsProperty,
                    Value =
                        Transitions()
                            .AddBrush(MaterialIcon.ForegroundProperty)
                            .AddBrush(MaterialIcon.BackgroundProperty)
                            .AddDouble(MaterialIcon.FontSizeProperty)
                            .AddDouble(MaterialIcon.HeightProperty)
                            .AddDouble(MaterialIcon.WidthProperty)
                            .AddDouble(MaterialIcon.OpacityProperty)
                )
            )

            style
        )

        resources |> Colors.create |> ButtonTheme.create |> WindowTheme.create |> ignore