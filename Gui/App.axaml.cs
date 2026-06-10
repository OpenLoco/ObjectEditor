using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Gui.ViewModels;
using Gui.Views;
using System;
using System.Collections.Generic;
using System.Threading;

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

			// The embedded ObjectService MUST NOT outlive the GUI under any circumstance
			// (clean close, OS shutdown, crash, force-kill). We hook every realistic exit
			// path; whichever fires first wins. ShutdownOnce makes the operation idempotent
			// so duplicate triggers are harmless.
			var shutdown = new ShutdownOnce(mainVm);

			desktop.ShutdownRequested += (_, _) => shutdown.Run();
			desktop.Exit += (_, _) => shutdown.Run();
			AppDomain.CurrentDomain.ProcessExit += (_, _) => shutdown.Run();
			AppDomain.CurrentDomain.UnhandledException += (_, _) => shutdown.Run();
		}

		base.OnFrameworkInitializationCompleted();
	}

	sealed class ShutdownOnce
	{
		readonly MainWindowViewModel mainVm;
		int ran;

		public ShutdownOnce(MainWindowViewModel mainVm) => this.mainVm = mainVm;

		public void Run()
		{
			if (Interlocked.Exchange(ref ran, 1) != 0)
			{
				return;
			}

			try
			{
				// Bound the wait so a misbehaving Kestrel stop can't hang the process
				// indefinitely. 5s is well above normal Kestrel drain time.
				var disposeTask = mainVm.EditorContext.DisposeAsync().AsTask();
				_ = disposeTask.Wait(TimeSpan.FromSeconds(5));
			}
			catch
			{
				// best-effort: never throw from shutdown
			}
		}
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
