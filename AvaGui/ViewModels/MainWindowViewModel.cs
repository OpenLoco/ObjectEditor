using AvaGui.Models;
using Avalonia;
using Avalonia.Platform;
using OpenLoco.Common;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
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
		public ILocoFileViewModel CurrentEditorModel { get; set; }

		public ObservableCollection<MenuItemViewModel> ObjDataItems { get; }

		public ObservableCollection<MenuItemViewModel> DataItems { get; init; }

		public ObservableCollection<LogLine> Logs => Model.LoggerObservableLogs;

		public ReactiveCommand<Unit, Unit> LoadPalette { get; }

		public ReactiveCommand<Unit, Unit> OpenSettingsFolder { get; }
		public ReactiveCommand<Unit, Task> OpenSingleObject { get; }

		public const string GithubApplicationName = "ObjectEditor";
		public const string GithubIssuePage = "https://github.com/OpenLoco/ObjectEditor/issues";
		public const string GithubLatestReleaseDownloadPage = "https://github.com/OpenLoco/ObjectEditor/releases";
		public const string GithubLatestReleaseAPI = "https://api.github.com/repos/OpenLoco/ObjectEditor/releases/latest";

		public string WindowTitle => $"{ObjectEditorModel.ApplicationName} - {ApplicationVersion} ({LatestVersionText})";

		[Reactive]
		public Version ApplicationVersion { get; set; }

		[Reactive]
		public string LatestVersionText { get; set; } = "Up-to-date";

		public MainWindowViewModel()
		{
			var paletteUri = new Uri("avares://ObjectEditor/Assets/palette.png");
			var palette = Image.Load<Rgb24>(AssetLoader.Open(paletteUri));

			Model = new()
			{
				PaletteMap = new PaletteMap(palette)
			};

			FolderTreeViewModel = new FolderTreeViewModel(Model);

			_ = FolderTreeViewModel.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(SetObjectViewModel);

			ObjDataItems = new ObservableCollection<MenuItemViewModel>(Model.Settings.ObjDataDirectories
				.Select(x => new MenuItemViewModel(
					x,
					ReactiveCommand.Create(() => FolderTreeViewModel.CurrentLocalDirectory = x))));
			ObjDataItems.Insert(0, new MenuItemViewModel("Add new folder", ReactiveCommand.Create(SelectNewFolder)));
			ObjDataItems.Insert(1, new MenuItemViewModel("-", ReactiveCommand.Create(() => { })));

			OpenSettingsFolder = ReactiveCommand.Create(() => PlatformSpecific.FolderOpenInDesktop(ObjectEditorModel.SettingsPath));
			OpenSingleObject = ReactiveCommand.Create(LoadSingleObjectToIndex);

			#region Version

			_ = this.WhenAnyValue(o => o.ApplicationVersion)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));
			_ = this.WhenAnyValue(o => o.LatestVersionText)
				.Subscribe(_ => this.RaisePropertyChanged(nameof(WindowTitle)));

			var assembly = Assembly.GetExecutingAssembly();
			ApplicationVersion = GetCurrentAppVersion(assembly);

			// check for new version
			var latestVersion = GetLatestAppVersion();
			if (latestVersion > ApplicationVersion)
			{
				LatestVersionText = $"newer version exists: {latestVersion}";
			}
			#endregion
		}

		void SetObjectViewModel(FileSystemItemBase? file)
		{
			if (file != null)
			{
				CurrentEditorModel = new ObjectEditorViewModel(file, Model);
			}
		}

		public async Task LoadSingleObjectToIndex()
		{
			var openFile = await PlatformSpecific.OpenFilePicker();
			if (openFile == null)
			{
				return;
			}

			var path = openFile.SingleOrDefault()?.Path.AbsolutePath;
			if (path == null)
			{
				return;
			}

			//logger?.Info($"Opening {path}");
			// todo: instead of using FileSystemItem.path, add a property IsOnline
			if (Model.TryLoadObject(new FileSystemItem(path, Path.GetFileName(path), true, FileLocation.Local), out var uiLocoFile))
			{
				Model.Logger.Warning($"Successfully loaded {path}");
				var file = new FileSystemItem(path, uiLocoFile!.DatFileInfo.S5Header.Name, uiLocoFile.DatFileInfo.S5Header.SourceGame == OpenLoco.Dat.Data.SourceGame.Vanilla, FileLocation.Local);
				SetObjectViewModel(file);
			}
			else
			{
				Model.Logger.Warning($"Unable to load {path}");
			}
		}

		static Version GetCurrentAppVersion(Assembly assembly)
		{
			// grab current appl version from assembly
			const string versionFilename = "AvaGui.version.txt";
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
			throw new Exception("Unable to get latest version");
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
