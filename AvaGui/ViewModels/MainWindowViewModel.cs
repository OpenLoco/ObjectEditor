global using HeaderIndex = System.Collections.Generic.Dictionary<string, AvaGui.Models.IndexObjectHeader>;
global using ObjectCache = System.Collections.Generic.Dictionary<string, AvaGui.Models.UiLocoFile>;
using Avalonia;
using AvaGui.Models;
using ReactiveUI;
using System;
using System.Reactive;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;

namespace AvaGui.ViewModels
{
	public class MenuItemModel : ReactiveObject
	{
		public MenuItemModel(string name, ICommand command)
		{
			Name = name;
			Command = command;
		}

		[Reactive] public string Name { get; set; }
		[Reactive] public ICommand Command { get; set; }
	}

	public class MainWindowViewModel : ViewModelBase
	{
		public ObjectEditorModel Model { get; }

		public FolderTreeViewModel FolderTreeViewModel { get; }

		public ObjectEditorViewModel ObjectEditorViewModel { get; }

		public ObservableCollection<MenuItemModel> ObjDataItems { get; set; }

		public ObservableCollection<MenuItemModel> DataItems { get; set; }

		public MainWindowViewModel()
		{
			Model = new();
			FolderTreeViewModel = new
				FolderTreeViewModel(Model);
			ObjectEditorViewModel = new ObjectEditorViewModel(Model);

			_ = FolderTreeViewModel.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => ObjectEditorViewModel.CurrentlySelectedObject = o);

			ObjDataItems = new ObservableCollection<MenuItemModel>(Model.Settings.ObjDataDirectories
				.Select(x => new MenuItemModel(
					x,
					ReactiveCommand.Create<string>(Model.LoadObjDirectory))));

			DataItems = new ObservableCollection<MenuItemModel>(Model.Settings.DataDirectories
				.Select(x => new MenuItemModel(
					x,
					ReactiveCommand.Create<string, bool>(Model.LoadDataDirectory))));

			//GenerateDataMenu = ReactiveCommand.CreateFromTask(...);
			//LoadPalette = ReactiveCommand.CreateFromTask(...); // nothing for now
			RecreateIndex = ReactiveCommand.CreateFromTask(async () => await Model.LoadObjDirectoryAsync(Model.Settings.ObjDataDirectory, null, false));
		}

		//public ReactiveCommand<Unit, Unit> ToggleThemeCommand { get; }
		//public ReactiveCommand<Unit, Unit> GenerateObjDataMenu { get; }
		//public ReactiveCommand<Unit, Unit> GenerateDataMenu { get; }

		public ReactiveCommand<Unit, Unit> LoadPalette { get; }
		public ReactiveCommand<Unit, Unit> RecreateIndex { get; }

		public bool IsDarkTheme
		{
			get => Application.Current.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
			set => Application.Current.RequestedThemeVariant = Application.Current.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark
				? Avalonia.Styling.ThemeVariant.Light
				: Avalonia.Styling.ThemeVariant.Dark;
		}
	}
}
