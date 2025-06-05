using Avalonia;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using DynamicData;
using NuGet.Versioning;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Gui.Models;
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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if !DEBUG
using OpenLoco.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
#endif

namespace OpenLoco.Gui.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public ObjectEditorModel Model { get; }

		public FolderTreeViewModel FolderTreeViewModel { get; }

		[Reactive]
		public TabViewPageViewModel CurrentTabModel { get; set; } = new();

		public ObservableCollection<MenuItemViewModel> ObjDataItems { get; init; } = [];

		public ObservableCollection<LogLine> Logs => Model.LoggerObservableLogs;

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
		public ReactiveCommand<Unit, Process?> OpenDownloadLink { get; }

		public const string GithubApplicationName = "ObjectEditor";
		public const string GithubIssuePage = "https://github.com/OpenLoco/ObjectEditor/issues";
		public const string GithubLatestReleaseDownloadPage = "https://github.com/OpenLoco/ObjectEditor/releases";
		public const string GithubLatestReleaseAPI = "https://api.github.com/repos/OpenLoco/ObjectEditor/releases/latest";

		public string WindowTitle => $"{ObjectEditorModel.ApplicationName} - {ApplicationVersion} ({LatestVersionText})";

		[Reactive]
		public SemanticVersion ApplicationVersion { get; set; }
		public static readonly SemanticVersion UnknownVersion = new(0, 0, 0, "unknown");

		[Reactive]
		public string LatestVersionText { get; set; } = "Development build";

		[Reactive]
		public bool IsUpdateAvailable { get; set; }

		const string DefaultPaletteImageString = "avares://ObjectEditor/Assets/palette.png";
		Image<Rgba32> DefaultPaletteImage { get; init; }

		public Interaction<EditorSettingsWindowViewModel, EditorSettingsWindowViewModel?> ShowDialog { get; }

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
			OpenSettingsFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(ObjectEditorModel.ProgramDataPath, Model.Logger));
			OpenG1 = ReactiveCommand.Create(LoadG1);
			OpenSCV5 = ReactiveCommand.Create(LoadSCV5);
			OpenMusic = ReactiveCommand.Create(LoadMusic);
			OpenSoundEffect = ReactiveCommand.Create(LoadSoundEffects);
			OpenTutorial = ReactiveCommand.Create(LoadTutorial);
			OpenScores = ReactiveCommand.Create(LoadScores);
			OpenLanguage = ReactiveCommand.Create(LoadLanguage);

			UseDefaultPalette = ReactiveCommand.Create(LoadDefaultPalette);
			UseCustomPalette = ReactiveCommand.Create(LoadCustomPalette);

			ShowDialog = new();
			EditSettingsCommand = ReactiveCommand.CreateFromTask(async () =>
			{
				var vm = new EditorSettingsWindowViewModel(Model.Settings);
				var result = await ShowDialog.Handle(vm);
				Model.Settings.Save(ObjectEditorModel.SettingsFile, Model.Logger);
			});

			OpenDownloadLink = ReactiveCommand.Create(() => Process.Start(new ProcessStartInfo(GithubLatestReleaseDownloadPage) { UseShellExecute = true }));

			#region Version

			_ = this.WhenAnyValue(o => o.ApplicationVersion)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));
			_ = this.WhenAnyValue(o => o.LatestVersionText)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));

			var assembly = Assembly.GetExecutingAssembly();

			ApplicationVersion = GetCurrentAppVersion(assembly);

#if !DEBUG

			try
			{
				var latestVersion = GetLatestAppVersion();
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

		void PopulateObjDataMenu()
		{
			ObjDataItems.Clear();

			ObjDataItems.Add(new MenuItemViewModel("Add new folder", ReactiveCommand.Create(SelectNewFolder)));
			ObjDataItems.Add(new MenuItemViewModel("-", ReactiveCommand.Create(() => { })));

			if (Directory.Exists(Model.Settings.AppDataObjDataFolder))
			{
				ObjDataItems.Add(new MenuItemViewModel($"[{nameof(GameObjDataFolder.AppData)}] {Model.Settings.AppDataObjDataFolder}", ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = Model.Settings.AppDataObjDataFolder)));
			}

			if (Directory.Exists(Model.Settings.LocomotionObjDataFolder))
			{
				ObjDataItems.Add(new MenuItemViewModel($"[{nameof(GameObjDataFolder.Locomotion)}] {Model.Settings.LocomotionObjDataFolder}", ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = Model.Settings.LocomotionObjDataFolder)));
			}

			if (Directory.Exists(Model.Settings.OpenLocoObjDataFolder))
			{
				ObjDataItems.Add(new MenuItemViewModel($"[{nameof(GameObjDataFolder.OpenLoco)}] {Model.Settings.OpenLocoObjDataFolder}", ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = Model.Settings.OpenLocoObjDataFolder)));
			}

			ObjDataItems.Add(new MenuItemViewModel("-", ReactiveCommand.Create(() => { })));

			// add the rest
			ObjDataItems.AddRange(
				Model.Settings.ObjDataDirectories
					.Select(x => new MenuItemViewModel(
						x,
						ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = x))));
		}

		public static async Task<FileSystemItemBase?> GetFileSystemItemFromUser(IReadOnlyList<FilePickerFileType> filetypes)
		{
			var openFile = await PlatformSpecific.OpenFilePicker(filetypes);
			if (openFile == null)
			{
				return null;
			}

			var path = openFile.SingleOrDefault()?.Path.LocalPath;

			return path == null
				? null
				: new FileSystemItemBase(path, Path.GetFileName(path), string.Empty, File.GetCreationTimeUtc(path), File.GetLastWriteTimeUtc(path), FileLocation.Local);
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

			var createdTime = File.GetCreationTimeUtc(fsi.Filename);
			var modifiedTime = File.GetLastWriteTimeUtc(fsi.Filename);

			if (Model.TryLoadObject(new FileSystemItemBase(fsi.Filename, Path.GetFileName(fsi.Filename), fsi.InternalName, createdTime, modifiedTime, FileLocation.Local), out var uiLocoFile) && uiLocoFile != null)
			{
				Model.Logger.Warning($"Successfully loaded {fsi.Filename}");
				var source = OriginalObjectFiles.GetFileSource(uiLocoFile.DatFileInfo.S5Header.Name, uiLocoFile.DatFileInfo.S5Header.Checksum);
				var fsi2 = new FileSystemItemBase(fsi.Filename, uiLocoFile!.DatFileInfo.S5Header.Name, fsi.InternalName, createdTime, modifiedTime, FileLocation.Local, source);
				SetObjectViewModel(fsi2);
			}
			else
			{
				Model.Logger.Warning($"Unable to load {fsi.Filename}");
			}
		}

		void SetObjectViewModel(FileSystemItemBase fsi)
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

		static SemanticVersion GetCurrentAppVersion(Assembly assembly)
		{
			// grab current app version from assembly
			const string versionFilename = "Gui.version.txt";
			using (var stream = assembly.GetManifestResourceStream(versionFilename))
			using (var ms = new MemoryStream())
			{
				stream!.CopyTo(ms);
				var versionText = Encoding.ASCII.GetString(ms.ToArray());
				return GetVersionFromText(versionText);
			}
		}

#if !DEBUG
		// thanks for this one @IntelOrca, https://github.com/IntelOrca/PeggleEdit/blob/master/src/peggleedit/Forms/MainMDIForm.cs#L848-L861
		SemanticVersion GetLatestAppVersion()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(GithubApplicationName, ApplicationVersion.ToString()));
			var response = client.GetAsync(GithubLatestReleaseAPI).Result;
			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = response.Content.ReadAsStringAsync().Result;
				var body = JsonSerializer.Deserialize<VersionCheckBody>(jsonResponse);
				var versionText = body?.TagName;
				return GetVersionFromText(versionText);
			}

#pragma warning disable CA2201 // Do not raise reserved exception types
			throw new Exception($"Unable to get latest version. Error={response.StatusCode}");
#pragma warning restore CA2201 // Do not raise reserved exception types
		}
#endif

		static SemanticVersion GetVersionFromText(string? versionText)
		{
			if (string.IsNullOrEmpty(versionText))
			{
				return UnknownVersion;
			}

			return
				SemanticVersion.TryParse(versionText.Trim(), out var version)
				? version
				: UnknownVersion;
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

			if (Model.Settings.AppDataObjDataFolder != dirPath)
			{
				Model.Logger.Warning("No need to add - this is the predefined AppData folder");
				return;
			}

			if (Model.Settings.LocomotionObjDataFolder != dirPath)
			{
				Model.Logger.Warning("No need to add - this is the predefined Locomotion ObjData folder");
				return;
			}

			if (Model.Settings.OpenLocoObjDataFolder != dirPath)
			{
				Model.Logger.Warning("No need to add - this is the predefined OpenLoco object folder");
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
}
