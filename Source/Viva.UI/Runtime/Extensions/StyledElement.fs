namespace Viva.UI.Runtime.Extensions

open System.Runtime.CompilerServices

open Avalonia
open Avalonia.Controls

type StyledElementExtensions =
    [<Extension>]
    static member RegisterIn(this: #StyledElement, namescope: #INameScope) =
        match this.Name with
        | null -> ()
        | name -> namescope.Register(name, this)

        this