using Avalonia;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using Common;
using Common.Logging;
using Dat.Data;
using Definitions.ObjectModels;
using DynamicData;
using Gui.Models;
using Gui.ViewModels.Loco.Tutorial;
using NuGet.Versioning;
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

namespace Gui.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public ObjectEditorContext EditorContext { get; }

	public FolderTreeViewModel FolderTreeViewModel { get; }

	[Reactive]
	public TabViewPageViewModel CurrentTabModel { get; set; } = new();

	public ObservableCollection<MenuItemViewModel> ObjDataItems { get; init; } = [];

	public ReactiveCommand<Unit, Unit> OpenCacheFolder { get; }
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
	public ReactiveCommand<Unit, Unit> DownloadLatestUpdate { get; }

	public string WindowTitle => $"{ObjectEditorContext.ApplicationName} - {ApplicationVersion} ({LatestVersionText})";

	[Reactive]
	public SemanticVersion ApplicationVersion { get; set; }
	SemanticVersion? LatestVersion { get; set; }

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

		EditorContext = new();
		Task.Run(LoadDefaultPalette);

		FolderTreeViewModel = new FolderTreeViewModel(EditorContext);

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

		EditorContext.Logger.LogAdded += (sender, laea) =>
		{
			if (EditorContext.Settings.ShowLogsOnError)
			{
				// announce to users that something bad happened
				var log = laea.Log;
				if (log.Level is LogLevel.Error)
				{
					// Dispatch the UI-related work to the UI thread.
					Avalonia.Threading.Dispatcher.UIThread.Post(() =>
					{
						// check if the logs window is already open
						if (App.GetOpenWindows().Any(x => x.DataContext is LogWindowViewModel))
						{
							return;
						}

						ShowLogsCommand.Execute();
					});
				}
			}
		};

		PopulateObjDataMenu();

		OpenSingleObject = ReactiveCommand.Create(LoadSingleObject);
		OpenCacheFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(EditorContext.Settings.CacheFolder, EditorContext.Logger));
		OpenDownloadFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(EditorContext.Settings.DownloadFolder, EditorContext.Logger));
		OpenSettingsFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(ObjectEditorContext.ProgramDataPath, EditorContext.Logger));
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
			var vm = new EditorSettingsWindowViewModel(EditorContext.Settings);
			var result = await OpenEditorSettingsWindow.Handle(vm);
			EditorContext.Settings.Save(ObjectEditorContext.SettingsFile, EditorContext.Logger);
		});

		OpenLogWindow = new();
		ShowLogsCommand = ReactiveCommand.CreateFromTask(async () =>
		{
			var vm = new LogWindowViewModel(EditorContext.LoggerObservableLogs);
			var result = await OpenLogWindow.Handle(vm);
		});

		#region Version

		_ = this.WhenAnyValue(o => o.ApplicationVersion)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));
		_ = this.WhenAnyValue(o => o.LatestVersionText)
			.Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));

		OpenDownloadLink = ReactiveCommand.Create(VersionHelpers.OpenDownloadPage);
		DownloadLatestUpdate = ReactiveCommand.Create(() =>
		{
			if (ApplicationVersion == null || ApplicationVersion == VersionHelpers.UnknownVersion)
			{
				EditorContext.Logger.Info($"{nameof(ApplicationVersion)} is null");
			}

			if (LatestVersion == null || LatestVersion == VersionHelpers.UnknownVersion)
			{
				EditorContext.Logger.Info($"{nameof(LatestVersion)} is null");
			}

			EditorContext.Logger.Info($"Attempting to update from {ApplicationVersion} to {LatestVersion}");
			var t = Task.Run(() => VersionHelpers.StartAutoUpdater(EditorContext.Logger, ApplicationVersion, LatestVersion));

		});

		ApplicationVersion = VersionHelpers.GetCurrentAppVersion();

#if !DEBUG
		try
		{
			LatestVersion = VersionHelpers.GetLatestAppVersion(VersionHelpers.ObjectEditorName);
			if (LatestVersion > ApplicationVersion)
			{
				LatestVersionText = $"newer version exists: {LatestVersion}";
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

	void PopulateObjDataMenu()
	{
		ObjDataItems.Clear();

		ObjDataItems.Add(new MenuItemViewModel("Add new folder", ReactiveCommand.Create(SelectNewFolder)));
		ObjDataItems.Add(new MenuItemViewModel("-", ReactiveCommand.Create(() => { })));

		if (Directory.Exists(EditorContext.Settings.AppDataObjDataFolder))
		{
			ObjDataItems.Add(new MenuItemViewModel($"[{nameof(GameObjDataFolder.AppData)}] {EditorContext.Settings.AppDataObjDataFolder}", ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = EditorContext.Settings.AppDataObjDataFolder)));
		}

		if (Directory.Exists(EditorContext.Settings.LocomotionSteamObjDataFolder))
		{
			ObjDataItems.Add(new MenuItemViewModel($"[{nameof(GameObjDataFolder.LocomotionSteam)}] {EditorContext.Settings.LocomotionSteamObjDataFolder}", ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = EditorContext.Settings.LocomotionSteamObjDataFolder)));
		}

		if (Directory.Exists(EditorContext.Settings.LocomotionGoGObjDataFolder))
		{
			ObjDataItems.Add(new MenuItemViewModel($"[{nameof(GameObjDataFolder.LocomotionGoG)}] {EditorContext.Settings.LocomotionGoGObjDataFolder}", ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = EditorContext.Settings.LocomotionGoGObjDataFolder)));
		}

		if (Directory.Exists(EditorContext.Settings.OpenLocoObjDataFolder))
		{
			ObjDataItems.Add(new MenuItemViewModel($"[{nameof(GameObjDataFolder.OpenLoco)}] {EditorContext.Settings.OpenLocoObjDataFolder}", ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = EditorContext.Settings.OpenLocoObjDataFolder)));
		}

		ObjDataItems.Add(new MenuItemViewModel("-", ReactiveCommand.Create(() => { })));

		// add the rest
		ObjDataItems.AddRange(
			EditorContext.Settings.ObjDataDirectories
				.Select(x => new MenuItemViewModel(
					x,
					ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = x))));
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
			EditorContext.PaletteMap = new PaletteMap(DefaultPaletteImage);
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

		EditorContext.PaletteMap = new PaletteMap(path);
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

		if (EditorContext.TryLoadObject(new FileSystemItem(Path.GetFileName(fsi.FileName), fsi.FileName, fsi.Id, createdTime, modifiedTime, FileLocation.Local), out var uiLocoFile) && uiLocoFile != null)
		{
			EditorContext.Logger.Warning($"Successfully loaded {fsi.FileName}");
			var source = OriginalObjectFiles.GetFileSource(uiLocoFile.DatInfo.S5Header.Name, uiLocoFile.DatInfo.S5Header.Checksum, uiLocoFile.DatInfo.S5Header.ObjectSource);
			var fsi2 = new FileSystemItem(uiLocoFile!.DatInfo.S5Header.Name, fsi.FileName, fsi.Id, createdTime, modifiedTime, FileLocation.Local, source);
			SetObjectViewModel(fsi2);
		}
		else
		{
			EditorContext.Logger.Warning($"Unable to load {fsi.FileName}");
		}
	}

	void SetObjectViewModel(FileSystemItem fsi)
	{
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new ObjectEditorViewModel(fsi, EditorContext));
		}
	}

	public async Task LoadG1()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new G1ViewModel(fsi, EditorContext));
		}
	}

	public async Task LoadSCV5()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.SCV5FileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new SCV5ViewModel(fsi, EditorContext));
		}
	}

	async Task LoadMusic()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new MusicViewModel(fsi, EditorContext));
		}
	}

	async Task LoadSoundEffects()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new SoundEffectsViewModel(fsi, EditorContext));
		}
	}

	async Task LoadTutorial()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new TutorialViewModel(fsi, EditorContext));
		}
	}

	async Task LoadScores()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new ScoresViewModel(fsi, EditorContext));
		}
	}

	async Task LoadLanguage()
	{
		var fsi = await GetFileSystemItemFromUser(PlatformSpecific.DatFileTypes);
		if (fsi != null && !CurrentTabModel.DocumentExistsWithFile(fsi))
		{
			CurrentTabModel.AddDocument(new LanguageFileViewModel(fsi, EditorContext));
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
			EditorContext.Logger.Warning("Directory doesn't exist");
			return;
		}

		if (EditorContext.Settings.ObjDataDirectories.Contains(dirPath))
		{
			EditorContext.Logger.Warning("Object directory is already in the list");
			return;
		}

		if (EditorContext.Settings.LocomotionSteamObjDataFolder == dirPath)
		{
			EditorContext.Logger.Warning("No need to add - this is the predefined Locomotion (Steam) ObjData folder");
			return;
		}

		if (EditorContext.Settings.LocomotionGoGObjDataFolder == dirPath)
		{
			EditorContext.Logger.Warning("No need to add - this is the predefined Locomotion (GoG) ObjData folder");
			return;
		}

		if (EditorContext.Settings.OpenLocoObjDataFolder == dirPath)
		{
			EditorContext.Logger.Warning("No need to add - this is the predefined OpenLoco object folder");
			return;
		}

		if (EditorContext.Settings.AppDataObjDataFolder == dirPath)
		{
			EditorContext.Logger.Warning("No need to add - this is the predefined AppData folder");
			return;
		}

		FolderTreeViewModel.CurrentLocalDirectory = dirPath; // this will cause the reindexing
		var menuItem = new MenuItemViewModel(
			dirPath,
			ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = dirPath)
			/*ReactiveCommand.Create(() => ObjDataItems.RemoveAt(ObjDataItems.Count))*/);

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
