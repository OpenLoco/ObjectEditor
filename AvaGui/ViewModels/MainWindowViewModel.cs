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
using OpenLoco.ObjectEditor;
using SixLabors.ImageSharp.PixelFormats;
using OpenLoco.ObjectEditor.Logging;

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

		public ObservableCollection<MenuItemModel> ObjDataItems { get; init; }

		public ObservableCollection<MenuItemModel> DataItems { get; init; }

		public ObservableCollection<LogLine> Logs => Model.LoggerObservableLogs;

		public MainWindowViewModel()
		{
			Model = new();
			FolderTreeViewModel = new FolderTreeViewModel(Model);
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
