using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Gui.ViewModels;
using Gui.Views;
using System.Collections.Generic;

namespace Gui;

public partial class App : Application
{
	public override void Initialize() => AvaloniaXamlLoader.Load(this);

	public override void OnFrameworkInitializationCompleted()
	{
		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			var mainVm = new MainWindowViewModel();
			desktop.MainWindow = new MainWindow
			{
				DataContext = mainVm,
			};

			// Ensure the embedded ObjectService (if started) and any other context-owned
			// resources are cleaned up when the application is shutting down.
			desktop.ShutdownRequested += (_, _) =>
			{
				try
				{
					mainVm.EditorContext.DisposeAsync().AsTask().GetAwaiter().GetResult();
				}
				catch
				{
					// best-effort: never throw from shutdown
				}
			};
		}

		base.OnFrameworkInitializationCompleted();
	}
	public static IEnumerable<Window> GetOpenWindows()
	{
		if (Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			return desktop.Windows;
		}

		return [];
	}
}
