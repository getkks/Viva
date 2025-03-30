namespace Viva.UI.Runtime

open Avalonia.Media

[<RequireQualifiedAccess>]
type Resource =
    | Background
    | Control
    | Disabled
    | Hover
    | Foreground

    override this.ToString() : string =
        match this with
        | Background -> "Background"
        | Control -> "Control"
        | Disabled -> "Disabled"
        | Hover -> "Hover"
        | Foreground -> "Foreground"

    member this.AsBrush = this :> IBrush IResource
    member this.AsColor = this :> Color IResource

    interface IBrush IResource with
        member this.Name =
            match this with
            | Background -> "BackgroundBrush"
            | Control -> "ControlBrush"
            | Disabled -> "DisabledBrush"
            | Hover -> "HoverBrush"
            | Foreground -> "ForegroundBrush"

        member _.Suffix = "Brush"

    interface Color IResource with
        member this.Name =
            match this with
            | Background -> "BackgroundColor"
            | Control -> "ControlColor"
            | Disabled -> "DisabledColor"
            | Hover -> "HoverColor"
            | Foreground -> "ForegroundColor"

        member _.Suffix = "Color"