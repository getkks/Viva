namespace Viva.UI

open Avalonia
open Avalonia.Controls
open Avalonia.Controls.ApplicationLifetimes
open Avalonia.Markup.Xaml
open Avalonia.Styling

open Material.Icons.Avalonia

open Elmish
open Avalonia.FuncUI.Elmish

// open Viva.UI.Styles
open Viva.UI.Controls.Host

type HostApplication() =
    inherit Application()
    // override this.Initialize() = AvaloniaXamlLoader.Load this

    override this.OnFrameworkInitializationCompleted() =
        match this.ApplicationLifetime with
        | :? IClassicDesktopStyleApplicationLifetime as desktopLifetime ->
            null |> MaterialIconStyles |> this.Styles.Add
            Theme.VivaTheme.create(this, this.Resources :?> ResourceDictionary)
            let mainWindow = Window()
            desktopLifetime.MainWindow <- mainWindow

            Program.mkProgram Window.init Window.update Window.view
            |> Program.withHost mainWindow
#if DEBUG
            |> Program.withConsoleTrace
#endif
            |> Program.runWith mainWindow
        | _ -> ()