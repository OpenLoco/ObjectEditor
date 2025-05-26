using Avalonia;
using Avalonia.Logging;
using Avalonia.ReactiveUI;
using System;

namespace OpenLoco.Gui
{
	class Program
	{
		// Initialization code. Don't use any Avalonia, third-party APIs or any
		// SynchronizationContext-reliant code before AppMain is called: things aren't initialized
		// yet and stuff might break.
		[STAThread]
		public static void Main(string[] args)
		{
			PreventRunningAsAdmin();
			_ = BuildAvaloniaApp()
				.StartWithClassicDesktopLifetime(args);
		}

		static void PreventRunningAsAdmin()
		{
			if (PlatformSpecific.RunningAsAdmin())
			{
				const string errorMessage = "This application should not be run with elevated privileges. Please run it as a regular user.";

				// show user a message. must be OS-independent
				Console.Error.WriteLine(errorMessage);

				// terminate current program
				throw new UnauthorizedAccessException(errorMessage);
			}
		}

		// Avalonia configuration, don't remove; also used by visual designer.
		public static AppBuilder BuildAvaloniaApp()
			=> AppBuilder.Configure<App>()
				.UsePlatformDetect()
				.WithInterFont()
				.LogToTrace(LogEventLevel.Verbose, LogArea.Binding)
				.UseReactiveUI();
	}
}
