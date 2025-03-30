namespace Viva.UI

module Program =
    open Avalonia

    [<CompiledName "BuildAvaloniaApp">]
    let buildAvaloniaApp() =
        AppBuilder.Configure<HostApplication>().UsePlatformDetect().WithInterFont().UseSkia().LogToTrace(areas = Array.empty)

    let main args =
        buildAvaloniaApp().StartWithClassicDesktopLifetime args