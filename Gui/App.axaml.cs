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
		var viewModel = new MainWindowViewModel();

		if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
		{
			desktop.MainWindow = new MainWindow
			{
				DataContext = viewModel,
			};
		}
		else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
		{
			singleView.MainView = new MainView
			{
				DataContext = viewModel,
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
