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
using System.Reactive.Linq;
using System.Diagnostics;

namespace AvaGui.ViewModels
{
	public class MenuItemModel : ReactiveObject
	{
		public MenuItemModel(string name, ICommand menuCommand)
		{
			Name = name;
			MenuCommand = menuCommand;
		}

		[Reactive] public string Name { get; set; }
		[Reactive] public ICommand MenuCommand { get; set; }
	}

	public class MainWindowViewModel : ViewModelBase
	{
		public ObjectEditorModel Model { get; }

		public FolderTreeViewModel FolderTreeViewModel { get; }

		public ObjectEditorViewModel ObjectEditorViewModel { get; }

		[Reactive] public ObservableCollection<MenuItemModel> ObjDataItems { get; set; }

		[Reactive] public ObservableCollection<MenuItemModel> DataItems { get; set; }

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

			//DataItems = new ObservableCollection<MenuItemModel>(Model.Settings.DataDirectories
			//	.Select(x => new MenuItemModel(
			//		x,
			//		ReactiveCommand.Create<string, bool>(Model.LoadDataDirectory))));

			RecreateIndex = ReactiveCommand.Create<string>((x) =>
			{
				Debug.WriteLine($"RecreateIndex {x} command was run.");
				Model.LoadObjDirectory(Model.Settings.ObjDataDirectory);
			});

			LoadPalette = ReactiveCommand.Create(() => Debug.WriteLine("LoadPalette command was run."));

			//LoadPalette = ReactiveCommand.CreateFromTask(...); // nothing for now
			//RecreateIndex = ReactiveCommand.CreateFromTask(async () => await Model.LoadObjDirectoryAsync(Model.Settings.ObjDataDirectory, null, false));
		}

		//public ReactiveCommand<Unit, Unit> ToggleThemeCommand { get; }
		//public ReactiveCommand<Unit, Unit> GenerateObjDataMenu { get; }
		//public ReactiveCommand<Unit, Unit> GenerateDataMenu { get; }

		public ReactiveCommand<Unit, Unit> LoadPalette { get; }
		public ReactiveCommand<string, Unit> RecreateIndex { get; }

		public bool IsDarkTheme
		{
			get => Application.Current.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
			set => Application.Current.RequestedThemeVariant = Application.Current.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark
				? Avalonia.Styling.ThemeVariant.Light
				: Avalonia.Styling.ThemeVariant.Dark;
		}
	}
}
