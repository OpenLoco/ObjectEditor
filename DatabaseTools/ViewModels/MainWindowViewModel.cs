using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using DatabaseTools.Models;
using DatabaseTools.Services;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;

namespace DatabaseTools.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
	public ToolsSettings Settings { get; } = ToolsSettings.Load(ToolsSettings.DefaultSettingsPath);

	public string SettingsPath => ToolsSettings.DefaultSettingsPath;

	public ObservableCollection<ScriptDescriptor> ExportScripts { get; }
	public ObservableCollection<ScriptDescriptor> ImportScripts { get; }
	public ObservableCollection<ScriptDescriptor> HelperScripts { get; }

	[Reactive] public string LogText { get; set; } = string.Empty;
	[Reactive] public bool IsRunning { get; set; }
	[Reactive] public string? CurrentScript { get; set; }

	public ReactiveCommand<ScriptDescriptor, Unit> RunScriptCommand { get; }
	public ReactiveCommand<Unit, Unit> ClearLogCommand { get; }
	public ReactiveCommand<Unit, Unit> SaveSettingsCommand { get; }
	public ReactiveCommand<Unit, Unit> BrowseDatabaseFileCommand { get; }
	public ReactiveCommand<Unit, Unit> BrowseObjectDirectoryCommand { get; }
	public ReactiveCommand<Unit, Unit> BrowseJsonDirectoryCommand { get; }

	public MainWindowViewModel()
	{
		ExportScripts =
		[
			new ScriptDescriptor(
				"Export all tables",
				"Exports authors, tags, licences, SC5 files and packs, object packs, and object metadata to JSON files in the JSON directory.",
				DatabaseExportService.ExportAllAsync),
		];

		ImportScripts =
		[
			new ScriptDescriptor(
				"Seed database",
				"Loads JSON files from the JSON directory and the object index from the object directory, then populates the database. Honours the 'Delete existing on import' flag.",
				DatabaseImportService.SeedAllAsync),
		];

		HelperScripts =
		[
			new("Query: Building var_AC", "Dumps Building var_AC values for Locomotion-Steam buildings to CSV.", DatabaseHelperScripts.QueryBuildingVar_AC),
			new("Query: Building produced quantity", "Dumps building produced cargo and quantities to CSV.", DatabaseHelperScripts.QueryBuildingProducedQuantity),
			new("Query: Headquarters buildings", "Logs buildings flagged as headquarters.", DatabaseHelperScripts.QueryHeadquarters),
			new("Query: Cost index (vanilla)", "Logs CostIndex on every vanilla object that exposes one.", DatabaseHelperScripts.QueryCostIndex),
			new("Query: TrackStation TrackPieces", "Logs TrackStation TrackPieces flags for every TrackStation object.", DatabaseHelperScripts.QueryTrackStationOneSidedTrack),
			new("Query: Industries with shadows", "Dumps industries with the HasShadows flag to CSV.", DatabaseHelperScripts.QueryIndustryHasShadows),
			new("Query: Vehicle alternating-car-sprite flag", "Dumps vehicles with the AlternatingCarSprite flag to CSV.", DatabaseHelperScripts.QueryVehicleBodyUnkSprites),
			new("Query: Cargo categories", "Dumps cargo objects, categories and English-UK names to CSV.", DatabaseHelperScripts.QueryCargoCategories),
			new("Query: All cost indices", "Dumps CostIndex / PaymentIndex / RunCostIndex on every object that has one.", DatabaseHelperScripts.QueryCostIndices),
			new("Write xxHash3 values", "Computes and stores xxHash3 for every DatObject in the database.", DatabaseHelperScripts.WritexxHash3),
			new("Fix object descriptions", "Re-reads each object's first English-UK string and overwrites the database description.", DatabaseHelperScripts.FixObjectDescriptions),
			new("Write StringTable", "Re-imports every object's StringTable rows into the database.", DatabaseHelperScripts.WriteStringTable),
			new("Setup sub-objects", "Walks every object and inserts/updates the typed sub-object tables (ObjClimate, etc).", DatabaseHelperScripts.SetupSubObjects),
			new("Query: Climate sub-objects", "Logs FirstSeason for every ObjClimate row.", DatabaseHelperScripts.QuerySubObjects),
		];

		RunScriptCommand = ReactiveCommand.CreateFromTask<ScriptDescriptor>(
			RunScriptAsync,
			this.WhenAnyValue(x => x.IsRunning).Select(b => !b));

		ClearLogCommand = ReactiveCommand.Create(() => { LogText = string.Empty; });

		SaveSettingsCommand = ReactiveCommand.Create(SaveSettings);
		BrowseDatabaseFileCommand = ReactiveCommand.CreateFromTask(BrowseDatabaseFileAsync);
		BrowseObjectDirectoryCommand = ReactiveCommand.CreateFromTask(BrowseObjectDirectoryAsync);
		BrowseJsonDirectoryCommand = ReactiveCommand.CreateFromTask(BrowseJsonDirectoryAsync);
	}

	void SaveSettings()
	{
		try
		{
			Settings.Save(ToolsSettings.DefaultSettingsPath);
			AppendLog($"Settings saved to {ToolsSettings.DefaultSettingsPath}");
		}
		catch (Exception ex)
		{
			AppendLog($"!!! Failed to save settings: {ex.Message}");
		}
	}

	static IStorageProvider? GetStorageProvider()
		=> Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop
			? desktop.MainWindow?.StorageProvider
			: null;

	async Task<string?> PickFolderAsync(string title, string? currentPath)
	{
		var provider = GetStorageProvider();
		if (provider == null)
		{
			return null;
		}

		IStorageFolder? startLocation = null;
		if (!string.IsNullOrWhiteSpace(currentPath) && Directory.Exists(currentPath))
		{
			startLocation = await provider.TryGetFolderFromPathAsync(currentPath);
		}

		var folders = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions
		{
			Title = title,
			AllowMultiple = false,
			SuggestedStartLocation = startLocation,
		});

		return folders.Count > 0 ? folders[0].TryGetLocalPath() : null;
	}

	async Task<string?> PickFileAsync(string title, string? currentPath, IReadOnlyList<FilePickerFileType> filetypes)
	{
		var provider = GetStorageProvider();
		if (provider == null)
		{
			return null;
		}

		IStorageFolder? startLocation = null;
		if (!string.IsNullOrWhiteSpace(currentPath))
		{
			var dir = Path.GetDirectoryName(currentPath);
			if (!string.IsNullOrEmpty(dir) && Directory.Exists(dir))
			{
				startLocation = await provider.TryGetFolderFromPathAsync(dir);
			}
		}

		var files = await provider.OpenFilePickerAsync(new FilePickerOpenOptions
		{
			Title = title,
			AllowMultiple = false,
			FileTypeFilter = filetypes,
			SuggestedStartLocation = startLocation,
		});

		return files.Count > 0 ? files[0].TryGetLocalPath() : null;
	}

	async Task BrowseDatabaseFileAsync()
	{
		var picked = await PickFileAsync(
			"Select database file",
			Settings.DatabaseFile,
			[new FilePickerFileType("SQLite database") { Patterns = ["*.db", "*.sqlite", "*.sqlite3"] }, FilePickerFileTypes.All]);
		if (!string.IsNullOrEmpty(picked))
		{
			Settings.DatabaseFile = picked;
		}
	}

	async Task BrowseObjectDirectoryAsync()
	{
		var picked = await PickFolderAsync("Select objects directory", Settings.ObjectDirectory);
		if (!string.IsNullOrEmpty(picked))
		{
			Settings.ObjectDirectory = picked;
		}
	}

	async Task BrowseJsonDirectoryAsync()
	{
		var picked = await PickFolderAsync("Select JSON directory", Settings.JsonDirectory);
		if (!string.IsNullOrEmpty(picked))
		{
			Settings.JsonDirectory = picked;
		}
	}

	async Task RunScriptAsync(ScriptDescriptor descriptor)
	{
		IsRunning = true;
		CurrentScript = descriptor.Name;
		AppendLog($"=== Running: {descriptor.Name} ===");

		var origOut = Console.Out;
		var origErr = Console.Error;
		var capture = new CallbackTextWriter(AppendLog);
		Console.SetOut(capture);
		Console.SetError(capture);

		try
		{
			await descriptor.Run(Settings, AppendLog);
			AppendLog($"=== Finished: {descriptor.Name} ===");
		}
		catch (Exception ex)
		{
			AppendLog($"!!! {descriptor.Name} failed: {ex}");
		}
		finally
		{
			capture.Flush();
			Console.SetOut(origOut);
			Console.SetError(origErr);
			IsRunning = false;
			CurrentScript = null;
		}
	}

	void AppendLog(string line)
	{
		if (Dispatcher.UIThread.CheckAccess())
		{
			LogText += line + Environment.NewLine;
		}
		else
		{
			Dispatcher.UIThread.Post(() => LogText += line + Environment.NewLine);
		}
	}
}
