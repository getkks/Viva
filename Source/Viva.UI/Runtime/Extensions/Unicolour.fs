namespace Viva.UI.Runtime

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open Avalonia.Media
open Wacton.Unicolour

[<Extension>]
type UnicolourExtensions() =
    [<Extension>]
    static member ToColor(this: Unicolour) =
        let struct (r, g, b) = this.Rgb.ConstrainedTriplet.Tuple
        Color.FromArgb(byte(this.Alpha.ConstrainedA * 255.), byte(r * 255.), byte(g * 255.), byte(b * 255.))

    [<Extension>]
    static member ToBrush(this: Unicolour) : IBrush = this.ToColor() |> SolidColorBrush :> _