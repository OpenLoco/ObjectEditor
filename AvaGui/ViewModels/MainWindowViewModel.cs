global using HeaderIndex = System.Collections.Generic.Dictionary<string, OpenLoco.ObjectEditor.AvaGui.Models.IndexObjectHeader>;
global using ObjectCache = System.Collections.Generic.Dictionary<string, OpenLoco.ObjectEditor.AvaGui.Models.UiLocoObject>;
using Avalonia;
using Avalonia.Interactivity;
using OpenLoco.ObjectEditor.AvaGui.Models;
using ReactiveUI;
using System;
using System.Reactive;
using System.Windows.Input;

namespace AvaGui.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public ObjectEditorModel Model { get; }

		public FolderTreeViewModel FolderTreeViewModel { get; }

		public ObjectEditorViewModel ObjectEditorViewModel { get; }

		public MainWindowViewModel()
		{
			Model = new();
			FolderTreeViewModel = new FolderTreeViewModel(Model);
			ObjectEditorViewModel = new ObjectEditorViewModel(Model);

			FolderTreeViewModel.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => ObjectEditorViewModel.CurrentlySelectedObject = o);

			ToggleThemeCommand = ReactiveCommand.Create(ToggleTheme);
			//themeToggle.Click += ToggleTheme;
			//RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Light;
		}

		public ReactiveCommand<Unit, Unit> ToggleThemeCommand { get; }

		private void ToggleTheme()
		{
			if (Application.Current.RequestedThemeVariant == Avalonia.Styling.ThemeVariant.Dark)
			{
				Application.Current.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Light;
			}
			else
			{
				Application.Current.RequestedThemeVariant = Avalonia.Styling.ThemeVariant.Dark;
			}
		}
	}
}
