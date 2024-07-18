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
using System.IO;
using System.Threading.Tasks;
using System.Reflection;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace AvaGui.ViewModels
{
	public class MenuItemModel(string name, ICommand menuCommand, ICommand deleteCommand) : ReactiveObject
	{
		[Reactive] public string Name { get; set; } = name;
		[Reactive] public ICommand MenuCommand { get; set; } = menuCommand;
		[Reactive] public ICommand DeleteCommand { get; set; } = deleteCommand;
	}

	public class MainWindowViewModel : ViewModelBase
	{
		public ObjectEditorModel Model { get; }

		public FolderTreeViewModel FolderTreeViewModel { get; }

		public ObjectEditorViewModel ObjectEditorViewModel { get; }
		public SCV5ViewModel SCV5ViewModel { get; }

		[Reactive]
		public object CurrentEditorModel { get; set; } // this will either be ObjectEditorViewModel for objects, or SCV5ViewModel for scenarios/landscapes/saves. in future, it'll also be different for g1.dat, tutorials, sfx files, etc

		public ObservableCollection<MenuItemModel> ObjDataItems { get; set; }

		public ObservableCollection<MenuItemModel> DataItems { get; init; }

		public ObservableCollection<LogLine> Logs => Model.LoggerObservableLogs;

		public ReactiveCommand<Unit, Unit> LoadPalette { get; }


		public ReactiveCommand<Unit, Unit> OpenSettingsFolder { get; }

		public const string GithubApplicationName = "ObjectEditor";
		public const string GithubIssuePage = "https://github.com/OpenLoco/ObjectEditor/issues";
		public const string GithubLatestReleaseDownloadPage = "https://github.com/OpenLoco/ObjectEditor/releases";
		public const string GithubLatestReleaseAPI = "https://api.github.com/repos/OpenLoco/ObjectEditor/releases/latest";

		public string WindowTitle => $"{ObjectEditorModel.ApplicationName} - {ApplicationVersion} ({LatestVersionText})";
		[Reactive] Version ApplicationVersion { get; set; }
		[Reactive] string LatestVersionText { get; set; } = "Up-to-date";


		//FileViewModel
		//- S5HeaderViewModel
		//- ObjectViewModel
		//- etc

		//ObjectViewModel
		//- ObjectData
		//- StringTableViewModel
		//- ImageTableViewModel

		//ObjectSelectorViewModel

		public MainWindowViewModel()
		{
			var paletteUri = new Uri("avares://ObjectEditor/Assets/palette.png");
			var palette = Image.Load<Rgb24>(AssetLoader.Open(paletteUri));

			Model = new()
			{
				PaletteMap = new PaletteMap(palette)
			};

			FolderTreeViewModel = new FolderTreeViewModel(Model);
			ObjectEditorViewModel = new ObjectEditorViewModel(Model);
			SCV5ViewModel = new SCV5ViewModel();

			_ = ObjectEditorViewModel.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(_ => CurrentEditorModel = ObjectEditorViewModel);

			_ = this.WhenAnyValue(o => o.ObjectEditorViewModel)
				.Subscribe(_ => CurrentEditorModel = ObjectEditorViewModel);
			_ = this.WhenAnyValue(o => o.SCV5ViewModel)
				.Subscribe(_ => CurrentEditorModel = SCV5ViewModel);

			_ = FolderTreeViewModel.WhenAnyValue(o => o.CurrentlySelectedObject)
				.Subscribe(o => ObjectEditorViewModel.CurrentlySelectedObject = o);

			ObjDataItems = new ObservableCollection<MenuItemModel>(Model.Settings.ObjDataDirectories
				.Select(x => new MenuItemModel(
					x,
					ReactiveCommand.Create(() => FolderTreeViewModel.CurrentDirectory = x),
					null)));
			ObjDataItems.Insert(0, new MenuItemModel("📂 Add new folder", ReactiveCommand.Create(SelectNewFolder), null));
			ObjDataItems.Insert(1, new MenuItemModel("-", ReactiveCommand.Create(() => { }), null));

			//DataItems = new ObservableCollection<MenuItemModel>(Model.Settings.DataDirectories
			//	.Select(x => new MenuItemModel(
			//		x,
			//		ReactiveCommand.Create<string, bool>(Model.LoadDataDirectory))));

			//DataItems.Insert(0, new MenuItemModel("Add new folder", ReactiveCommand.Create(() => { })));
			//DataItems.Insert(1, new MenuItemModel("-", ReactiveCommand.Create(() => { })));

			//LoadPalette = ReactiveCommand.Create(LoadPaletteFunc);
			OpenSettingsFolder = ReactiveCommand.Create(PlatformSpecific.FolderOpenInDesktop);

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
				//_ = MessageBox.Show($"Current Version: {ApplicationVersion}{Environment.NewLine}Latest version: {latestVersion}{Environment.NewLine}Taking you to the downloads page now ");
				//_ = Process.Start(new ProcessStartInfo { FileName = GithubLatestReleaseDownloadPage, UseShellExecute = true });
				LatestVersionText = $"newer version exists: {latestVersion}";
			}
			#endregion
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
			if (Directory.Exists(dirPath) && Directory.EnumerateFiles(dirPath).Any() && !Model.Settings.ObjDataDirectories.Contains(dirPath))
			{
				await Model.LoadObjDirectoryAsync(dirPath, null, false);
				var menuItem = new MenuItemModel(
					dirPath,
					ReactiveCommand.Create(() => FolderTreeViewModel.CurrentDirectory = dirPath),
					ReactiveCommand.Create(() => ObjDataItems.RemoveAt(ObjDataItems.Count)));
				//menuItem.DeleteCommand = ReactiveCommand.Create(() =>
				//{
				//	_ = ObjDataItems.Remove(menuItem);
				//});

				ObjDataItems.Add(menuItem);
			}
		}

		//public void LoadPaletteFunc()
		//{
		//	using (var openFileDialog = new OpenFileDialog())
		//	{
		//		openFileDialog.InitialDirectory = lastPaletteDirectory;
		//		openFileDialog.Filter = "Palette Image Files(*.png)|*.png|All files (*.*)|*.*";
		//		openFileDialog.FilterIndex = 1;
		//		openFileDialog.RestoreDirectory = true;

		//		if (openFileDialog.ShowDialog() == DialogResult.OK && File.Exists(openFileDialog.FileName))
		//		{
		//			model.PaletteFile = openFileDialog.FileName;
		//			var paletteBitmap = SixLabors.ImageSharp.Image.Load<Rgb24>(openFileDialog.FileName);
		//			Model.PaletteMap = new PaletteMap(paletteBitmap);

		//			RefreshObjectUI();
		//			lastPaletteDirectory = Path.GetDirectoryName(openFileDialog.FileName) ?? lastPaletteDirectory;
		//		}
		//	}
		//}


		public static bool IsDarkTheme
		{
			get => Application.Current?.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark;
			set => Application.Current.RequestedThemeVariant = Application.Current.ActualThemeVariant == Avalonia.Styling.ThemeVariant.Dark
				? Avalonia.Styling.ThemeVariant.Light
				: Avalonia.Styling.ThemeVariant.Dark;
		}
	}
}
