namespace Viva.UI.Controls

[<AutoOpen>]
module MaterialIcon =
    open System
    open Avalonia
    open Avalonia.Controls
    open Avalonia.FuncUI.Types
    open Avalonia.FuncUI.Builder
    open Avalonia.FuncUI.DSL
    open Avalonia.FuncUI.Elmish
    open Avalonia.FuncUI.Hosts
    open Material.Icons.Avalonia

    /// <summary>Creates a new <see cref="T:Material.Icons.Avalonia.MaterialIcon"/> instance.</summary>
    /// <param name="attrs">The attributes of the <see cref="T:Material.Icons.Avalonia.MaterialIcon"/> instance.</param>
    /// <returns>A new <see cref="T:Material.Icons.Avalonia.MaterialIcon"/> instance.</returns>
    let create attrs : MaterialIcon IView = ViewBuilder.Create attrs

    type MaterialIcon with
        /// <summary> Gets or sets the icon. </summary>
        /// <param name="value">The icon.</param>
        /// <returns>An <see cref="T:Avalonia.FuncUI.Types.IAttr`1"/> that sets the icon.</returns>
        /// <typeparam name="a">Either <see cref="T:Material.Icons.Avalonia.MaterialIcon"/> or a derived type.</typeparam>
        static member kind value : #MaterialIcon IAttr =
            AttrBuilder.CreateProperty(MaterialIcon.KindProperty, value, ValueNone)