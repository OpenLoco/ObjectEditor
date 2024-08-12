using System.IO;
using System;
using OpenLoco.ObjectEditor.Settings;
using System.Text.Json;
using OpenLoco.ObjectEditor.Logging;
using Zenith.Core;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Objects;
using System.Threading;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor;
using System.Collections.ObjectModel;

namespace AvaGui.Models
{
	public class ObjectEditorModel
	{
		public EditorSettings Settings { get; private set; }

		public string SettingsFilePath { get; set; }

		public ILogger Logger;

		public HeaderIndex HeaderIndex { get; private set; } = [];

		public PaletteMap PaletteMap { get; set; }

		public G1Dat? G1 { get; set; }

		public Dictionary<string, byte[]> Music { get; } = [];

		public Dictionary<string, byte[]> MiscellaneousTracks { get; } = [];

		public Dictionary<string, byte[]> SoundEffects { get; } = [];

		public Dictionary<string, byte[]> Tutorials { get; } = [];

		public Collection<string> MiscFiles { get; } = [];

		public const string ApplicationName = "OpenLoco Object Editor";

		public static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

		public static string SettingsFile => Path.Combine(SettingsPath, "settings.json");

		public ObservableCollection<LogLine> LoggerObservableLogs = [];

		public ObjectEditorModel()
		{
			Logger = new Logger();
			LoggerObservableLogs = [];
			Logger.LogAdded += (sender, laea) => LoggerObservableLogs.Add(laea.Log);

			_ = Task.Run(() => LoadSettingsAsync(SettingsFile, Logger));
		}

		public async Task LoadSettingsAsync(string settingsFile, ILogger? logger)
		{
			SettingsFilePath = settingsFile;

			if (!File.Exists(settingsFile))
			{
				Settings = new();
				await SaveSettingsAsync();
				return;
			}

			var text = await File.ReadAllTextAsync(settingsFile);
			var settings = JsonSerializer.Deserialize<EditorSettings>(text);
			Verify.NotNull(settings);

			Settings = settings!;

			if (!ValidateSettings(Settings, logger))
			{
				return;
			}

			if (File.Exists(Settings.GetObjDataFullPath(Settings.IndexFileName)))
			{
				logger?.Info($"Loading header index from \"{Settings.IndexFileName}\"");
				await LoadObjDirectoryAsync(Settings.ObjDataDirectory, new Progress<float>(), true);
			}
		}

		static bool ValidateSettings(EditorSettings settings, ILogger? logger)
		{
			if (settings == null)
			{
				logger?.Error("Invalid settings file: Unable to deserialise settings file");
				return false;
			}

			if (string.IsNullOrEmpty(settings.ObjDataDirectory))
			{
				logger?.Warning("Invalid settings file: Object directory was null or empty");
				return false;
			}

			if (!Directory.Exists(settings.ObjDataDirectory))
			{
				logger?.Warning($"Invalid settings file: Directory \"{settings.ObjDataDirectory}\" does not exist");
				return false;
			}

			return true;
		}

		public async Task SaveSettingsAsync()
		{
			var options = GetOptions();
			var text = JsonSerializer.Serialize(Settings, options);

			var parentDir = Path.GetDirectoryName(SettingsFilePath);
			if (parentDir != null && !Directory.Exists(parentDir))
			{
				_ = Directory.CreateDirectory(parentDir);
			}

			await File.WriteAllTextAsync(SettingsFilePath, text);
		}

		public bool TryLoadObject(string filename, out UiLocoFile? uiLocoFile)
		{
			if (string.IsNullOrEmpty(filename))
			{
				uiLocoFile = null;
				return false;
			}

			DatFileInfo? fileInfo;
			ILocoObject? locoObject;

			try
			{
				(fileInfo, locoObject) = SawyerStreamReader.LoadFullObjectFromFile(filename, logger: Logger);
			}
			catch (Exception ex)
			{
				Logger?.Error($"Unable to load {filename}", ex);
				uiLocoFile = null;
				return false;
			}

			if (locoObject == null || fileInfo == null)
			{
				Logger?.Error($"Unable to load {filename}. FileInfo={fileInfo}");
				uiLocoFile = null;
				return false;
			}

			uiLocoFile = new UiLocoFile() { DatFileInfo = fileInfo, LocoObject = locoObject };
			return true;
		}

		// this method loads every single object entirely. it takes a long time to run
		async Task CreateIndex(string[] allFiles, IProgress<float> progress)
		{
			Logger?.Info($"Creating index on {allFiles.Length} files");

			var sw = new Stopwatch();
			sw.Start();

			var fileCount = allFiles.Length;
			var index = await SawyerStreamReader.FastIndexAsync(allFiles, progress);

			HeaderIndex = index.ToDictionary(
				x => x.filename,
				x => new ObjectIndexModel(
					x.s5.Name,
					DatFileType.Object,
					x.s5.ObjectType,
					x.s5.SourceGame,
					x.s5.Checksum,
					x.VehicleType)
				);

			sw.Stop();
			Logger?.Info($"Indexed {fileCount} in {sw.Elapsed}");
		}

		public void SaveFile(string path, UiLocoFile obj)
		{
			if (obj == null)
			{
				Logger?.Error("Cannot save an object with a null loco object - the file would be empty!");
				return;
			}

			SawyerStreamWriter.Save(path, obj.DatFileInfo.S5Header.Name, obj.LocoObject);
		}

		public bool LoadDataDirectory(string directory)
		{
			if (!Directory.Exists(directory))
			{
				Logger?.Warning("Invalid directory: doesn't exist");
				return false;
			}

			Settings.DataDirectory = directory;

			var allDataFiles = Directory.GetFiles(Settings.DataDirectory).Select(f => Path.GetFileName(f).ToLower()).ToHashSet();

			void LoadKnownData(HashSet<string> allFilesInDir, HashSet<string> knownFilenames, Dictionary<string, byte[]> dict)
			{
				dict.Clear();
				var expectedMusicFiles = knownFilenames.Select(f => f.ToLower());
				foreach (var music in expectedMusicFiles)
				{
					var matching = allFilesInDir.Where(f => f.EndsWith(music));
					if (matching.Any())
					{
						dict.Add(music, File.ReadAllBytes(Path.Combine(Settings.DataDirectory, music)));
						_ = allFilesInDir.RemoveWhere(f => f.EndsWith(music));
					}
				}
			}

			LoadKnownData(allDataFiles, [.. OriginalDataFiles.Music.Keys], Music);
			LoadKnownData(allDataFiles, [.. OriginalDataFiles.MiscellaneousTracks.Keys], MiscellaneousTracks);
			LoadKnownData(allDataFiles, [OriginalDataFiles.SoundEffect], SoundEffects);
			LoadKnownData(allDataFiles, OriginalDataFiles.Tutorials, Tutorials);

			//MiscFiles = [.. allDataFiles];

			// load G1 only for now since we need it for palette
			G1 = SawyerStreamReader.LoadG1(Settings.GetDataFullPath(Settings.G1DatFileName));

			//LoadPalette(); // update palette from g1

			//await SaveSettings();

			return true;
		}

		// this method will load any supported file type
		//public void LoadDirectory(string directory)
		//{
		//	var allFiles = Directory.GetFiles(directory, "*.dat|*.sv5|*.sc5", SearchOption.AllDirectories);
		//}

		//public void LoadObjDirectory(string directory)
		//	=> LoadObjDirectory(directory, new Progress<float>(), true);

		public async Task LoadObjDirectoryAsync(string directory, IProgress<float> progress, bool useExistingIndex)
		{
			if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory) || progress == null)
			{
				Logger?.Error($"Couldn't start loading obj dir: {directory}");
				return;
			}

			Settings.ObjDataDirectory = directory;
			var allFiles = Directory
				.GetFiles(directory, "*", SearchOption.AllDirectories); // the searchPattern doesn't support full regex and is not case sensitive on windows but is case sensitive on linux

			allFiles = allFiles
				.Where(x => Path.GetExtension(x).Equals(".dat", StringComparison.OrdinalIgnoreCase))
				.ToArray();

			if (useExistingIndex && File.Exists(Settings.GetObjDataFullPath(Settings.IndexFileName)))
			{
				HeaderIndex = DeserialiseHeaderIndexFromFile(Settings.GetObjDataFullPath(Settings.IndexFileName)) ?? HeaderIndex;

				var a = HeaderIndex.Keys.Except(allFiles);
				var b = allFiles.Except(HeaderIndex.Keys);
				if (a.Any() || b.Any())
				{
					Logger?.Warning("Selected directory had an index file but it was outdated; suggest recreating it when you have a moment");
				}
			}
			else
			{
				Logger?.Info("Recreating index file");
				await CreateIndex(allFiles, progress); // do we need the array?
				SerialiseHeaderIndexToFile(Settings.GetObjDataFullPath(Settings.IndexFileName), HeaderIndex, GetOptions());
			}

			await SaveSettingsAsync();
		}

		private static JsonSerializerOptions GetOptions()
			=> new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, };

		static void SerialiseHeaderIndexToFile(string filename, HeaderIndex headerIndex, JsonSerializerOptions options, ILogger? logger = null)
		{
			logger?.Info($"Saved settings to {filename}");
			var json = JsonSerializer.Serialize(headerIndex, options);
			File.WriteAllText(filename, json);
		}

		static HeaderIndex? DeserialiseHeaderIndexFromFile(string filename, ILogger? logger = null)
		{
			if (!File.Exists(filename))
			{
				logger?.Info($"Settings file {filename} does not exist");
				return null;
			}

			logger?.Info($"Loading settings from {filename}");

			var json = File.ReadAllText(filename);

			return JsonSerializer.Deserialize<HeaderIndex>(json, GetOptions()) ?? [];
		}
	}
}
