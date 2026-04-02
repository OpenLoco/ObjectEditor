using System.Runtime.Versioning;
using Avalonia.Browser;

[assembly: SupportedOSPlatform("browser")]

await Gui.Program.BuildAvaloniaApp()
    .StartBrowserAppAsync("out");
