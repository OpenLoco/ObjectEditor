using Avalonia;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using DynamicData;
using NuGet.Versioning;
using Dat;
using Dat.Data;
using Gui.Models;
using PropertyModels.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;

#if !DEBUG
using Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
#endif

namespace Gui.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public ObjectEditorModel Model { get; }

	public FolderTreeViewModel FolderTreeViewModel { get; }

	[Reactive]
	public TabViewPageViewModel CurrentTabModel { get; set; } = new();

	public ObservableCollection<MenuItemViewModel> GameObjDataItems { get; init; } = [];
	public ObservableCollection<MenuItemViewModel> ObjDataItems { get; init; } = [];

	public ReactiveCommand<Unit, Unit> OpenDownloadFolder { get; }
	public ReactiveCommand<Unit, Unit> OpenSettingsFolder { get; }
	public ReactiveCommand<Unit, Task> OpenSingleObject { get; }
	public ReactiveCommand<Unit, Task> OpenG1 { get; }
	public ReactiveCommand<Unit, Task> OpenSCV5 { get; }
	public ReactiveCommand<Unit, Task> OpenMusic { get; }
	public ReactiveCommand<Unit, Task> OpenSoundEffect { get; }
	public ReactiveCommand<Unit, Task> OpenTutorial { get; }
	public ReactiveCommand<Unit, Task> OpenScores { get; }
	public ReactiveCommand<Unit, Task> OpenLanguage { get; }
	public ReactiveCommand<Unit, Task> UseDefaultPalette { get; }
	public ReactiveCommand<Unit, Task> UseCustomPalette { get; }
	public ReactiveCommand<Unit, Unit> EditSettingsCommand { get; }
	public ReactiveCommand<Unit, Unit> ShowLogsCommand { get; }
	public ReactiveCommand<Unit, Process?> OpenDownloadLink { get; }

	public string WindowTitle => $"{Definitions.Constants.ApplicationName} - {ApplicationVersion} ({LatestVersionText})";

	[Reactive]
	public SemanticVersion ApplicationVersion { get; set; }

	[Reactive]
	public string LatestVersionText { get; set; } = "Development build";

	[Reactive]
	public bool IsUpdateAvailable { get; set; }

	const string DefaultPaletteImageString = "avares://ObjectEditor/Assets/palette.png";
	Image<Rgba32> DefaultPaletteImage { get; init; }

	public Interaction<EditorSettingsWindowViewModel, EditorSettingsWindowViewModel?> OpenEditorSettingsWindow { get; }

	public Interaction<LogWindowViewModel, LogWindowViewModel?> OpenLogWindow { get; }

	public MainWindowViewModel()
	{
		DefaultPaletteImage = Image.Load<Rgba32>(AssetLoader.Open(new Uri(DefaultPaletteImageString)));

		Model = new();
		Task.Run(LoadDefaultPalette);

		FolderTreeViewModel = new FolderTreeViewModel(Model);

		_ = FolderTreeViewModel.WhenAnyValue(o => o.CurrentlySelectedObject)
			.Subscribe((x) =>
			{
				if (x != null && (x.SubNodes == null || x.SubNodes?.Count == 0))
				{
					SetObjectViewModel(x);
				}
			});

		_ = CurrentTabModel.WhenAnyValue(o => o.SelectedDocument)
			.Subscribe((x) => FolderTreeViewModel.CurrentlySelectedObject = x?.CurrentFile);

		PopulateObjDataMenu();

		OpenSingleObject = ReactiveCommand.Create(LoadSingleObject);
		OpenDownloadFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(Model.Settings.DownloadFolder, Model.Logger));
		OpenSettingsFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(Definitions.Constants.ProgramDataPath, Model.Logger));
		OpenG1 = ReactiveCommand.Create(LoadG1);
		OpenSCV5 = ReactiveCommand.Create(LoadSCV5);
		OpenMusic = ReactiveCommand.Create(LoadMusic);
		OpenSoundEffect = ReactiveCommand.Create(LoadSoundEffects);
		OpenTutorial = ReactiveCommand.Create(LoadTutorial);
		OpenScores = ReactiveCommand.Create(LoadScores);
		OpenLanguage = ReactiveCommand.Create(LoadLanguage);

		UseDefaultPalette = ReactiveCommand.Create(LoadDefaultPalette);
		UseCustomPalette = ReactiveCommand.Create(LoadCustomPalette);

		OpenEditorSettingsWindow = new();
		EditSettingsCommand = ReactiveCommand.CreateFromTask(async () =>
		{
			var vm = new EditorSettingsWindowViewModel(Model.Settings);
			var result = await OpenEditorSettingsWindow.Handle(vm);
			Model.Settings.Save(Definitions.Constants.EditorSettingsFile, Model.Logger);
		});

		OpenLogWindow = new();
		ShowLogsCommand = ReactiveCommand.CreateFromTask(async () =>
		{
			var vm = new LogWindowViewModel(Model.LoggerObservableLogs);
			var result = await OpenLogWindow.Handle(vm);
		});

		OpenDownloadLink = ReactiveCommand.Create(VersionHelpers.OpenDownloadPage);

		#region Version

		_ = this.WhenAnyValue(o => o.ApplicationVersion)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));
		_ = this.WhenAnyValue(o => o.LatestVersionText)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));

		ApplicationVersion = VersionHelpers.GetCurrentAppVersion();

#if !DEBUG
		try
		{
			var latestVersion = VersionHelpers.GetLatestAppVersion(ApplicationVersion);
			if (latestVersion > ApplicationVersion)
			{
				LatestVersionText = $"newer version exists: {latestVersion}";
				IsUpdateAvailable = true;
			}
			else
			{
				LatestVersionText = "latest version";
				IsUpdateAvailable = false;
			}
		}
		catch (Exception ex)
		{
			Model.Logger.Error(ex);
		}
#endif

		#endregion
	}

	void RemoveMenuItem(string name)
	{
		var item = ObjDataItems.FirstOrDefault(x => x.Name == name);
		if (item != null)
		{
			_ = ObjDataItems.Remove(item);
		}

		_ = Model.Settings.ObjDataDirectories.Remove(name);
		Model.Settings.Save(Definitions.Constants.EditorSettingsFile, Model.Logger);
	}

	ReactiveCommand<Unit, Unit> CreateDeleteCommand(string name)
		=> ReactiveCommand.Create(() => RemoveMenuItem(name));

	void PopulateObjDataMenu()
	{
		GameObjDataItems.Clear();
		ObjDataItems.Clear();

		var noOpCommand = ReactiveCommand.Create(() => { });
		AddObjectFolder(GameObjDataItems, Model.Settings.AppDataObjDataFolder, "FolderAccount", GameObjDataFolder.AppData);
		AddObjectFolder(GameObjDataItems, Model.Settings.LocomotionObjDataFolder, "Train", GameObjDataFolder.Locomotion);
		AddObjectFolder(GameObjDataItems, Model.Settings.OpenLocoObjDataFolder, "TrainVariant", GameObjDataFolder.OpenLoco);
		AddObjectFolder(GameObjDataItems, Model.Settings.DownloadFolder, "FolderDownload", GameObjDataFolder.Downloads);

		void AddObjectFolder(ObservableCollection<MenuItemViewModel> items, string folder, string iconName, GameObjDataFolder type)
		{
			if (Directory.Exists(folder))
			{
				items.Add(new MenuItemViewModel(
				$"[{type}] {folder}",
				ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = folder),
				iconName,
				noOpCommand));
			}
		}

		ObjDataItems.AddRange(
			Model.Settings.ObjDataDirectories
				.Select(x => new MenuItemViewModel(
					x,
					ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = x),
					"AccountBox",
					CreateDeleteCommand(x))));
	}

	public static async Task<FileSystemItem?> GetFileSystemItemFromUser(IReadOnlyList<FilePickerFileType> filetypes)
	{
		var openFile = await PlatformSpecific.OpenFilePicker(filetypes);
		if (openFile == null)
		{
			return null;
		}

		var path = openFile.SingleOrDefault()?.Path.LocalPath;

		return path == null
			? null
			: new FileSystemItem(Path.GetFileName(path), path, null, DateOnly.FromDateTime(File.GetCreationTimeUtc(path)), DateOnly.FromDateTime(File.GetLastWriteTimeUtc(path)), FileLocation.Local);
	}

	async Task LoadDefaultPalette()
		=> await Task.Run(() =>
		{
			Model.PaletteMap = new PaletteMap(DefaultPaletteImage);
			CurrentTabModel.ReloadAll();
		});

	async Task LoadCustomPalette()
	{
		// file picker
		var openFile = await PlatformSpecific.OpenFilePicker(PlatformSpecific.PngFileTypes);
		if (openFile == null)
		{
			return;
		}

		var path = openFile.SingleOrDefault()?.Path.LocalPath;
		if (path == null)
		{
			return;
		}

		Model.PaletteMap = new PaletteMap(path);
		CurrentTabModel.ReloadAll();
	}

	public async Task LoadSingleObject()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi == null)
		{
			return;
		}

		var createdTime = DateOnly.FromDateTime(File.GetCreationTimeUtc(fsi.FileName));
		var modifiedTime = DateOnly.FromDateTime(File.GetLastWriteTimeUtc(fsi.FileName));

		if (Model.TryLoadObject(new FileSystemItem(Path.GetFileName(fsi.FileName), fsi.FileName, fsi.Id, createdTime, modifiedTime, FileLocation.Local), out var uiLocoFile) && uiLocoFile != null)
		{
			Model.Logger.Warning($"Successfully loaded {fsi.FileName}");
			var source = OriginalObjectFiles.GetFileSource(uiLocoFile.DatFileInfo.S5Header.Name, uiLocoFile.DatFileInfo.S5Header.Checksum);
			var fsi2 = new FileSystemItem(uiLocoFile!.DatFileInfo.S5Header.Name, fsi.FileName, fsi.Id, createdTime, modifiedTime, FileLocation.Local, source);
			SetObjectViewModel(fsi2);
		}
		else
		{
			Model.Logger.Warning($"Unable to load {fsi.FileName}");
		}
	}

	void SetObjectViewModel(FileSystemItem fsi)
	{
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new DatObjectEditorViewModel(fsi, Model));
		}
	}

	public async Task LoadG1()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new G1ViewModel(fsi, Model));
		}
	}

	public async Task LoadSCV5()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.SCV5FileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new SCV5ViewModel(fsi, Model));
		}
	}

	async Task LoadMusic()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new MusicViewModel(fsi, Model));
		}
	}

	async Task LoadSoundEffects()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new SoundEffectsViewModel(fsi, Model));
		}
	}

	async Task LoadTutorial()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new TutorialViewModel(fsi, Model));
		}
	}

	async Task LoadScores()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new ScoresViewModel(fsi, Model));
		}
	}

	async Task LoadLanguage()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new LanguageViewModel(fsi, Model));
		}
	}

	public async Task SelectNewFolder()
	{
		var folders = await PlatformSpecific.OpenFolderPicker();
		var dir = folders.FirstOrDefault();
		if (dir == null)
		{
			return;
		}

		var dirPath = dir.Path.LocalPath;

		if (!Directory.Exists(dirPath))
		{
			Model.Logger.Warning("Directory doesn't exist");
			return;
		}

		if (Model.Settings.ObjDataDirectories.Contains(dirPath))
		{
			Model.Logger.Warning("Object directory is already in the list");
			return;
		}

		if (Model.Settings.AppDataObjDataFolder == dirPath)
		{
			Model.Logger.Warning("No need to add - this is the predefined AppData folder");
			return;
		}

		if (Model.Settings.LocomotionObjDataFolder == dirPath)
		{
			Model.Logger.Warning("No need to add - this is the predefined Locomotion ObjData folder");
			return;
		}

		if (Model.Settings.OpenLocoObjDataFolder == dirPath)
		{
			Model.Logger.Warning("No need to add - this is the predefined OpenLoco object folder");
			return;
		}

		FolderTreeViewModel.CurrentLocalDirectory = dirPath; // this will cause the reindexing
		var menuItem = new MenuItemViewModel(
			dirPath,
			ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = dirPath),
			"AccountBox",
			CreateDeleteCommand(dirPath));

		ObjDataItems.Add(menuItem);
	}

	public static bool IsDarkTheme
	{
		get => Application.Current?.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
		set => Application.Current!.RequestedThemeVariant = Application.Current.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark
			? Avalonia.Styling.ThemeVariant.Light
			: Avalonia.Styling.ThemeVariant.Dark;
	}
}
