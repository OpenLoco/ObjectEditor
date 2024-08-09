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
using ReactiveUI;

namespace AvaGui.Models
{
	public enum DatFileType
	{
		Object,
		Scenario,
		SaveGame,
		Tutorial,
		G1,
		Music,
		SoundEffect, // css*.dat
		Language,
		Scores,
	}

	public class ObjectEditorModel
	{
		public EditorSettings Settings { get; private set; }

		public string SettingsFilePath { get; set; }

		public ILogger Logger;

		public HeaderIndex HeaderIndex { get; private set; } = [];

		public PaletteMap PaletteMap { get; set; }

		public G1Dat? G1 { get; set; }

		public Dictionary<string, byte[]> Music { get; set; } = [];

		public Dictionary<string, byte[]> MiscellaneousTracks { get; set; } = [];

		public Dictionary<string, byte[]> SoundEffects { get; set; } = [];

		public Dictionary<string, byte[]> Tutorials { get; set; } = [];

		public Collection<string> MiscFiles { get; set; } = [];

		public const string ApplicationName = "OpenLoco Object Editor";

		public static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

		public static string SettingsFile => Path.Combine(SettingsPath, "settings.json");

		public ObservableCollection<LogLine> LoggerObservableLogs = new();

		public ObjectEditorModel()
		{
			Logger = new Logger();
			LoggerObservableLogs = new ObservableCollection<LogLine>(((Logger)Logger).Logs);

			LoadSettings(SettingsFile, Logger);
		}

		public void LoadSettings(string settingsFile, ILogger? logger)
		{
			SettingsFilePath = settingsFile;

			if (!File.Exists(settingsFile))
			{
				Settings = new();
				SaveSettings();
				return;
			}

			var text = File.ReadAllText(settingsFile);
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
				LoadObjDirectory(Settings.ObjDataDirectory, new Progress<float>(), true);
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

		public void SaveSettings()
		{
			var options = GetOptions();
			var text = JsonSerializer.Serialize(Settings, options);

			var parentDir = Path.GetDirectoryName(SettingsFilePath);
			if (parentDir != null && !Directory.Exists(parentDir))
			{
				_ = Directory.CreateDirectory(parentDir);
			}

			File.WriteAllText(SettingsFilePath, text);
		}

		public bool TryLoadObject(string filename, out UiLocoFile? uiLocoFile)
		{
			if (string.IsNullOrEmpty(filename))
			{
				uiLocoFile = null;
				return false;
			}

			(var fileInfo, var locoObject) = SawyerStreamReader.LoadFullObjectFromFile(filename, logger: Logger);

			if (locoObject == null)
			{
				Logger?.Error($"Unable to load {filename}. FileInfo={fileInfo}");
				uiLocoFile = null;
				return false;
			}

			uiLocoFile = new UiLocoFile() { DatFileInfo = fileInfo, LocoObject = locoObject };
			return true;
		}

		// this method loads every single object entirely. it takes a long time to run
		void CreateIndex(string[] allFiles, IProgress<float>? progress)
		{
			Logger?.Info($"Creating index on {allFiles.Length} files");

			ConcurrentDictionary<string, IndexObjectHeader> ccHeaderIndex = new(); // key is full path/filename

			var count = 0;
			ConcurrentDictionary<string, TimeSpan> timePerFile = new();

			var sw = new Stopwatch();
			sw.Start();

			var fileCount = allFiles.Length;
			var parallelise = true; // todo: remove this or make a user setting

			if (parallelise)
			{
				_ = Parallel.ForEach(allFiles, new ParallelOptions() { MaxDegreeOfParallelism = 16 }, (filename)
					=> count = LoadAndIndexFile(count, filename));
			}
			else
			{
				foreach (var filename in allFiles)
				{
					count = LoadAndIndexFile(count, filename);
				}
			}

			HeaderIndex = ccHeaderIndex.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			sw.Stop();
			Logger?.Info("Finished creating index");
			Logger?.Debug($"Time time={sw.Elapsed}");

			if (timePerFile.IsEmpty)
			{
				_ = timePerFile.TryAdd("<no items>", TimeSpan.Zero);
			}

			var slowest = timePerFile.MaxBy(x => x.Value.Ticks);
			Logger?.Debug($"Slowest file={slowest.Key} Time={slowest.Value}");

			var average = timePerFile.Average(x => x.Value.TotalMilliseconds);
			Logger?.Debug($"Average time={average}ms");

			var median = timePerFile.OrderBy(x => x.Value).Skip(timePerFile.Count / 2).Take(1).Single();
			Logger?.Debug($"Median time={median.Value}ms");

			int LoadAndIndexFile(int count, string filename)
			{
				var startTime = sw.Elapsed;
				var loadResult = TryLoadObject(filename, out var uiLocoFile);
				var elapsed = sw.Elapsed - startTime;

				if (loadResult && uiLocoFile != null)
				{
					_ = ccHeaderIndex.TryAdd(filename, new IndexObjectHeader(
						uiLocoFile.DatFileInfo.S5Header.Name,
						DatFileType.Object,
						uiLocoFile.DatFileInfo.S5Header.ObjectType,
						uiLocoFile.DatFileInfo.S5Header.SourceGame,
						uiLocoFile.DatFileInfo.S5Header.Checksum,
						uiLocoFile.LocoObject.Object is VehicleObject veh ? veh.Type : null));

					_ = timePerFile.TryAdd(uiLocoFile.DatFileInfo.S5Header.Name, elapsed);
				}
				else
				{
					Logger?.Error($"Failed to load \"{filename}\"");
				}

				_ = Interlocked.Increment(ref count);
				progress?.Report((float)count / fileCount);
				return count;
			}
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

			MiscFiles = [.. allDataFiles];

			// load G1 only for now since we need it for palette
			G1 = SawyerStreamReader.LoadG1(Settings.GetDataFullPath(Settings.G1DatFileName));

			//LoadPalette(); // update palette from g1

			SaveSettings();

			return true;
		}

		// this method will load any supported file type
		//public void LoadDirectory(string directory)
		//{
		//	var allFiles = Directory.GetFiles(directory, "*.dat|*.sv5|*.sc5", SearchOption.AllDirectories);
		//}

		public async Task LoadObjDirectoryAsync(string directory, IProgress<float>? progress, bool useExistingIndex)
		{
			await Task.Run(() => LoadObjDirectory(directory, progress, useExistingIndex));
			await Task.Run(SaveSettings);
		}

		public void LoadObjDirectory(string directory)
			=> LoadObjDirectory(directory, null, true);

		public void LoadObjDirectory(string directory, IProgress<float>? progress, bool useExistingIndex)
		{
			if (!Directory.Exists(directory))
			{
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
				CreateIndex(allFiles, progress); // do we need the array?
				SerialiseHeaderIndexToFile(Settings.GetObjDataFullPath(Settings.IndexFileName), HeaderIndex, GetOptions());
			}

			SaveSettings();
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
