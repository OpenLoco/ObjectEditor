global using HeaderIndex = System.Collections.Generic.Dictionary<string, OpenLoco.ObjectEditor.AvaGui.Models.IndexObjectHeader>;
global using ObjectCache = System.Collections.Generic.Dictionary<string, OpenLoco.ObjectEditor.AvaGui.Models.UiLocoObject>;
using Avalonia;
using OpenLoco.ObjectEditor.AvaGui.Models;
using ReactiveUI;
using System;
using System.Reactive;

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

			_ = FolderTreeViewModel.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => ObjectEditorViewModel.CurrentlySelectedObject = o);
		}

		public ReactiveCommand<Unit, Unit> ToggleThemeCommand { get; }

		public bool IsDarkTheme
		{
			get => Application.Current.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
			set => Application.Current.RequestedThemeVariant = Application.Current.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark
				? Avalonia.Styling.ThemeVariant.Light
				: Avalonia.Styling.ThemeVariant.Dark;
		}
	}
}
