namespace Viva.UI.Controls

open System
open System.Collections.Generic

open Avalonia
open Avalonia.Controls
open Avalonia.Interactivity
open Avalonia.Layout
open Avalonia.Media

open Avalonia.FuncUI
open Avalonia.FuncUI.DSL
open Avalonia.FuncUI.Types
open Viva.UI
open Viva.UI.Runtime

module CaptionButtons =
    open Wacton.Unicolour
    open Material.Icons
    open Material.Icons.Avalonia

    let minimizeBrush = Unicolour(ColourSpace.Oklch, 0.5, 0.103, 80).ToBrush()
    let maximizeBrush = Unicolour(ColourSpace.Oklch, 0.5, 0.172862, 142).ToBrush()
    let closeBrush = Unicolour(ColourSpace.Oklch, 0.5, 0.194, 26).ToBrush()

    let getWindow(args: RoutedEventArgs) =
        match args.Source with
        | :? Button as control ->
            match TopLevel.GetTopLevel control with
            | :? Window as window -> ValueSome window
            | _ -> ValueNone
        | _ -> ValueNone

    type CaptionButton =
        | Close
        | Maximize
        | Minimize
        | None

        member this.Brush =
            match this with
            | Close -> closeBrush
            | Maximize -> maximizeBrush
            | Minimize -> minimizeBrush
            | None -> Brushes.Transparent

        member this.MaterialKind(window: Window voption) =
            match this with
            | Close -> MaterialIconKind.Close
            | Maximize ->
                window
                |> ValueOption.map(fun window ->
                    if window.WindowState = WindowState.Maximized then
                        MaterialIconKind.WindowRestore
                    else
                        MaterialIconKind.Maximize
                )
                |> ValueOption.defaultValue MaterialIconKind.Maximize
            | Minimize -> MaterialIconKind.Minimize
            | None -> MaterialIconKind.Minimize

    let ifActiveToBrush (activeButton: CaptionButton IWritable) (condition: CaptionButton) =
        if activeButton.Current = condition then
            condition.Brush
        else
            Brushes.Transparent

    let renderCaptionButton (window: Window voption) (captionButton: CaptionButton) (activeButton: CaptionButton IWritable) : IView =
        Button.create [
            captionButton |> ifActiveToBrush activeButton |> Button.background
            Button.padding(10, 5)
            if activeButton.Current = captionButton then
                match captionButton with
                | Close -> Button.cornerRadius(CornerRadius(0, 0, 0, 5))
                | _ -> Button.cornerRadius(CornerRadius(0, 0, 5, 5))
            Button.content(MaterialIcon.create [ MaterialIcon.kind(captionButton.MaterialKind window) ])
            Button.onClick(fun args ->
                args.Handled <- true

                window
                |> ValueOption.iter(fun window ->
                    match captionButton with
                    | Close -> window.Close()
                    | Maximize ->
                        window.WindowState <-
                            if window.WindowState = WindowState.Maximized then
                                WindowState.Normal
                            else
                                WindowState.Maximized
                    | Minimize -> window.WindowState <- WindowState.Minimized
                    | None -> ()
                )
            )
            Button.onPointerEntered(fun args ->
                args.Handled <- true
                activeButton.Set captionButton
            )
            Button.onPointerExited(fun args ->
                args.Handled <- true
                activeButton.Set None
            )
        ]

    let renderCaptionButtons window activeButton captionButtons =
        captionButtons
        |> List.map(fun captionButton -> renderCaptionButton window captionButton activeButton)

    let create titleBarControls =
        Component.create(
            "",
            fun ctx ->
                let activeButton = ctx.useState None
                let window = ctx.control |> TopLevel.GetTopLevel |> unbox<Window> |> ValueOption.ofObj

                Border.create [
                    Border.background Brushes.Transparent
                    if window.IsSome && window.Value.WindowState <> WindowState.Maximized then
                        Border.borderBrush activeButton.Current.Brush
                        Border.borderThickness 1
                    else
                        Border.borderBrush Brushes.Transparent

                    Border.verticalAlignment VerticalAlignment.Stretch
                    Border.horizontalAlignment HorizontalAlignment.Stretch
                    Border.child(
                        StackPanel.create [
                            StackPanel.horizontalAlignment HorizontalAlignment.Right
                            StackPanel.verticalAlignment VerticalAlignment.Top
                            StackPanel.orientation Orientation.Horizontal
                            StackPanel.children [
                                yield! titleBarControls
                                yield!
                                    renderCaptionButtons window activeButton [
                                        Minimize
                                        Maximize
                                        Close
                                    ]
                            ]

                        ]
                    )
                ]
        )