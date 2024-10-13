using AvaGui.Models;
using Avalonia;
using Avalonia.Platform;
using OpenLoco.Common;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive;
using System.Reactive.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AvaGui.ViewModels
{
	public class MainWindowViewModel : ViewModelBase
	{
		public ObjectEditorModel Model { get; }

		public FolderTreeViewModel FolderTreeViewModel { get; }

		public SCV5ViewModel SCV5ViewModel { get; }

		[Reactive]
		public ILocoFileViewModel? CurrentEditorModel { get; set; } = null;

		public ObservableCollection<MenuItemViewModel> ObjDataItems { get; }

		public ObservableCollection<MenuItemViewModel> DataItems { get; init; }

		public ObservableCollection<LogLine> Logs => Model.LoggerObservableLogs;

		//public ReactiveCommand<Unit, Unit> LoadPalette { get; }

		public ReactiveCommand<Unit, Unit> OpenDownloadFolder { get; }
		public ReactiveCommand<Unit, Unit> OpenSettingsFolder { get; }
		public ReactiveCommand<Unit, Task> OpenSingleObject { get; }
		public ReactiveCommand<Unit, Task> OpenG1 { get; }

		public ReactiveCommand<Unit, Task> UseDefaultPalette { get; }
		public ReactiveCommand<Unit, Task> UseCustomPalette { get; }

		public const string GithubApplicationName = "ObjectEditor";
		public const string GithubIssuePage = "https://github.com/OpenLoco/ObjectEditor/issues";
		public const string GithubLatestReleaseDownloadPage = "https://github.com/OpenLoco/ObjectEditor/releases";
		public const string GithubLatestReleaseAPI = "https://api.github.com/repos/OpenLoco/ObjectEditor/releases/latest";

		public string WindowTitle => $"{ObjectEditorModel.ApplicationName} - {ApplicationVersion} ({LatestVersionText})";

		[Reactive]
		public Version ApplicationVersion { get; set; }

		[Reactive]
		public string LatestVersionText { get; set; } = "Up-to-date";

		const string DefaultPaletteImageString = "avares://ObjectEditor/Assets/palette.png";
		Image<Rgba32> DefaultPaletteImage { get; init; }

		public MainWindowViewModel()
		{
			DefaultPaletteImage = Image.Load<Rgba32>(AssetLoader.Open(new Uri(DefaultPaletteImageString)));

			Model = new();
			LoadDefaultPalette();

			FolderTreeViewModel = new FolderTreeViewModel(Model);

			_ = FolderTreeViewModel.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe((x) =>
				{
					if (x is FileSystemItemObject fsi)
					{
						SetObjectViewModel(fsi);
					}
				});

			ObjDataItems = new ObservableCollection<MenuItemViewModel>(Model.Settings.ObjDataDirectories
				.Select(x => new MenuItemViewModel(
					x,
					ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = x))));
			ObjDataItems.Insert(0, new MenuItemViewModel("Add new folder", ReactiveCommand.Create(SelectNewFolder)));
			ObjDataItems.Insert(1, new MenuItemViewModel("-", ReactiveCommand.Create(() => { })));

			OpenSingleObject = ReactiveCommand.Create(LoadSingleObject);
			OpenDownloadFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(Model.Settings.DownloadFolder));
			OpenSettingsFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(ObjectEditorModel.SettingsPath));
			OpenG1 = ReactiveCommand.Create(LoadG1);

			UseDefaultPalette = ReactiveCommand.Create(LoadDefaultPalette);
			UseCustomPalette = ReactiveCommand.Create(LoadCustomPalette);

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
				}
			}
			catch (Exception ex)
			{
				Model.Logger.Error(ex);
			}
#endif

			#endregion
		}

		async Task LoadDefaultPalette()
		{
			Model.PaletteMap = new PaletteMap(DefaultPaletteImage);
			if (CurrentEditorModel != null)
			{
				_ = await CurrentEditorModel.ReloadCommand.Execute();
			}
		}

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

			//
			Model.PaletteMap = new PaletteMap(path);
			// could use reactive here, but its simple for now so we won't. just reload the current model, which will in turn reload the images with the new palette
			if (CurrentEditorModel != null)
			{
				_ = await CurrentEditorModel.ReloadCommand.Execute();
			}
		}

		public async Task LoadSingleObject()
		{
			var openFile = await PlatformSpecific.OpenFilePicker(PlatformSpecific.DatFileTypes);
			if (openFile == null)
			{
				return;
			}

			var path = openFile.SingleOrDefault()?.Path.LocalPath;
			if (path == null)
			{
				return;
			}

			if (Model.TryLoadObject(new FileSystemItem(path, Path.GetFileName(path), FileLocation.Local), out var uiLocoFile) && uiLocoFile != null)
			{
				Model.Logger.Warning($"Successfully loaded {path}");
				var source = OriginalObjectFiles.GetFileSource(uiLocoFile.DatFileInfo.S5Header.Name, uiLocoFile.DatFileInfo.S5Header.Checksum);
				var fsi = new FileSystemItemObject(path, uiLocoFile!.DatFileInfo.S5Header.Name, FileLocation.Local, source);
				SetObjectViewModel(fsi);
			}
			else
			{
				Model.Logger.Warning($"Unable to load {path}");
			}
		}

		void SetObjectViewModel(FileSystemItemObject fsi) => CurrentEditorModel = new DatObjectEditorViewModel(fsi, Model);

		public async Task LoadG1()
		{
			var openFile = await PlatformSpecific.OpenFilePicker(PlatformSpecific.DatFileTypes);
			if (openFile == null)
			{
				return;
			}

			var path = openFile.SingleOrDefault()?.Path.LocalPath;
			if (path == null)
			{
				return;
			}

			var fsi = new FileSystemItem(path, Path.GetFileName(path), FileLocation.Local);
			SetG1ViewModel(fsi);
		}

		public void SetG1ViewModel(FileSystemItem fsi) => CurrentEditorModel = new G1ViewModel(fsi, Model);

		static Version GetCurrentAppVersion(Assembly assembly)
		{
			// grab current appl version from assembly
			const string versionFilename = "Gui.version.txt";
			using (var stream = assembly.GetManifestResourceStream(versionFilename))
			{
				var buf = new byte[5];
				var arr = stream!.Read(buf);
				return Version.Parse(Encoding.ASCII.GetString(buf));
			}
		}

		// thanks for this one @IntelOrca, https://github.com/IntelOrca/PeggleEdit/blob/master/src/peggleedit/Forms/MainMDIForm.cs#L848-L861
		Version GetLatestAppVersion()
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(GithubApplicationName, ApplicationVersion.ToString()));
			var response = client.GetAsync(GithubLatestReleaseAPI).Result;
			if (response.IsSuccessStatusCode)
			{
				var jsonResponse = response.Content.ReadAsStringAsync().Result;
				var body = JsonSerializer.Deserialize<VersionCheckBody>(jsonResponse);
				var tagName = body?.TagName;
				if (tagName != null)
				{
					return Version.Parse(tagName);
				}
			}

#pragma warning disable CA2201 // Do not raise reserved exception types
			throw new Exception($"Unable to get latest version. Error={response.StatusCode}");
#pragma warning restore CA2201 // Do not raise reserved exception types
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
			if (Directory.Exists(dirPath) && !Model.Settings.ObjDataDirectories.Contains(dirPath))
			{
				FolderTreeViewModel.CurrentLocalDirectory = dirPath; // this will cause the reindexing
				var menuItem = new MenuItemViewModel(
					dirPath,
					ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = dirPath)
					/*ReactiveCommand.Create(() => ObjDataItems.RemoveAt(ObjDataItems.Count))*/);

				ObjDataItems.Add(menuItem);
			}
		}

		public static bool IsDarkTheme
		{
			get => Application.Current?.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
			set => Application.Current.RequestedThemeVariant = Application.Current.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark
				? Avalonia.Styling.ThemeVariant.Light
				: Avalonia.Styling.ThemeVariant.Dark;
		}
	}
}
