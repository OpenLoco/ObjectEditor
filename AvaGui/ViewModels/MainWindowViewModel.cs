global using HeaderIndex = System.Collections.Generic.Dictionary<string, AvaGui.Models.IndexObjectHeader>;
global using ObjectCache = System.Collections.Generic.Dictionary<string, AvaGui.Models.UiLocoFile>;
using Avalonia;
using AvaGui.Models;
using ReactiveUI;
using System;
using System.Reactive;
using System.Linq;
using System.Windows.Input;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using OpenLoco.ObjectEditor;
using SixLabors.ImageSharp.PixelFormats;
using OpenLoco.ObjectEditor.Logging;
using Avalonia.Platform;
using SixLabors.ImageSharp;

namespace AvaGui.ViewModels
{
	public class MenuItemModel(string name, ICommand menuCommand) : ReactiveObject
	{
		[Reactive] public string Name { get; set; } = name;
		[Reactive] public ICommand MenuCommand { get; set; } = menuCommand;
	}

	public class MainWindowViewModel : ViewModelBase
	{
		public ObjectEditorModel Model { get; }

		public FolderTreeViewModel FolderTreeViewModel { get; }

		public ObjectEditorViewModel ObjectEditorViewModel { get; }

		public ObservableCollection<MenuItemModel> ObjDataItems { get; init; }

		public ObservableCollection<MenuItemModel> DataItems { get; init; }

		public ObservableCollection<LogLine> Logs => Model.LoggerObservableLogs;

//		//
//		FileViewModel
//- S5HeaderViewModel
//- ObjectViewModel
//- etc

//ObectViewModel
//- ObjectData
//- StringTableViewModel
//- ImageTableViewModel

//ObjectSelectorViewModel

		public MainWindowViewModel()
		{
			var paletteUri = new Uri("avares://AvaGui/Assets/palette.png");
			var palette = Image.Load<Rgb24>(AssetLoader.Open(paletteUri));

			Model = new();
			Model.PaletteMap = new PaletteMap(palette);

			FolderTreeViewModel = new FolderTreeViewModel(Model);
			ObjectEditorViewModel = new ObjectEditorViewModel(Model);

			_ = FolderTreeViewModel.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => ObjectEditorViewModel.CurrentlySelectedObject = o);

			ObjDataItems = new ObservableCollection<MenuItemModel>(Model.Settings.ObjDataDirectories
				.Select(x => new MenuItemModel(
					x,
					ReactiveCommand.Create<string>(Model.LoadObjDirectory))));

			ObjDataItems.Insert(0, new MenuItemModel("Add new folder", ReactiveCommand.Create(() => { })));
			ObjDataItems.Insert(1, new MenuItemModel("--------", ReactiveCommand.Create(() => { })));

			DataItems = new ObservableCollection<MenuItemModel>(Model.Settings.DataDirectories
				.Select(x => new MenuItemModel(
					x,
					ReactiveCommand.Create<string, bool>(Model.LoadDataDirectory))));

			DataItems.Insert(0, new MenuItemModel("Add new folder", ReactiveCommand.Create(() => { })));
			DataItems.Insert(1, new MenuItemModel("--------", ReactiveCommand.Create(() => { })));

			LoadPalette = ReactiveCommand.Create(LoadPaletteFunc);
			RecreateIndex = ReactiveCommand.Create(() => Model.LoadObjDirectory(Model.Settings.ObjDataDirectory, null, false));
		}

		public ReactiveCommand<Unit, Unit> LoadPalette { get; }

		public void LoadPaletteFunc()
		{
			//using (var openFileDialog = new OpenFileDialog())
			{
				//openFileDialog.InitialDirectory = lastPaletteDirectory;
				//openFileDialog.Filter = "Palette Image Files(*.png)|*.png|All files (*.*)|*.*";
				//openFileDialog.FilterIndex = 1;
				//openFileDialog.RestoreDirectory = true;

				//if (openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(openFileDialog.FileName))
				{
					//model.PaletteFile = openFileDialog.FileName;
					//var paletteBitmap = SixLabors.ImageSharp.Image.Load<Rgb24>(openFileDialog.FileName);
					//Model.PaletteMap = new PaletteMap(paletteBitmap);

					//RefreshObjectUI();
					//lastPaletteDirectory = Path.GetDirectoryName(openFileDialog.FileName) ?? lastPaletteDirectory;
				}
			}
		}

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
