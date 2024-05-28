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
using OpenLoco.ObjectEditor.Types;

namespace AvaGui.Models
{
	public class ObjectEditorModel
	{
		public EditorSettings Settings { get; private set; }

		public string SettingsFilePath { get; set; }

		public ILogger logger;

		public HeaderIndex HeaderIndex { get; private set; } = [];

		public ObjectCache ObjectCache { get; private set; } = [];

		public PaletteMap PaletteMap { get; set; }

		public G1Dat? G1 { get; set; }

		public Dictionary<string, byte[]> Music { get; set; } = [];

		public Dictionary<string, byte[]> MiscellaneousTracks { get; set; } = [];

		public Dictionary<string, byte[]> SoundEffects { get; set; } = [];

		public Dictionary<string, byte[]> Tutorials { get; set; } = [];

		public List<string> MiscFiles { get; set; } = [];

		const string GithubApplicationName = "ObjectEditor";
		const string GithubLatestReleaseDownloadPage = @"https://github.com/OpenLoco/ObjectEditor/releases";
		const string GithubLatestReleaseAPI = @"https://api.github.com/repos/OpenLoco/ObjectEditor/releases/latest";
		public const string ApplicationName = "OpenLoco Object Editor";

		string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
		string SettingsFile => Path.Combine(SettingsPath, "settings.json");

		public ObjectEditorModel()
		{
			LoadSettings(SettingsFile, logger);
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
			if (!Directory.Exists(parentDir))
			{
				_ = Directory.CreateDirectory(parentDir);
			}

			File.WriteAllText(SettingsFilePath, text);
		}

		// this method loads every single object entirely. it takes a long time to run
		void CreateIndex(string[] allFiles, IProgress<float>? progress)
		{
			ConcurrentDictionary<string, IndexObjectHeader> ccHeaderIndex = new(); // key is full path/filename
			ConcurrentDictionary<string, UiLocoFile> ccObjectCache = new(); // key is full path/filename

			var count = 0;

			ConcurrentDictionary<string, TimeSpan> timePerFile = new();

			logger?.Info($"Creating index on {allFiles.Length} files");
			var sw = new Stopwatch();
			sw.Start();

			_ = Parallel.ForEach(allFiles, new ParallelOptions() { MaxDegreeOfParallelism = 100 }, (file) =>
			//foreach (var file in allFiles)
			{
				try
				{
					var startTime = sw.Elapsed;
					var (fileInfo, locoObject) = SawyerStreamReader.LoadFullObjectFromFile(file);

					if (locoObject == null)
					{
						logger?.Error($"Unable to load {file}. FileInfo={fileInfo}");
						return;
					}

					if (!ccObjectCache.TryAdd(file, new UiLocoFile { DatFileInfo = fileInfo, LocoObject = locoObject }))
					{
						logger?.Warning($"Didn't add file {file} to cache - already exists (how???)");
					}

					VehicleType? veh = null;
					if (locoObject.Object is VehicleObject vo)
					{
						veh = vo.Type;
					}

					var indexObjectHeader = new IndexObjectHeader(fileInfo.S5Header.Name, fileInfo.S5Header.ObjectType, fileInfo.S5Header.Checksum, veh);
					if (!ccHeaderIndex.TryAdd(file, indexObjectHeader))
					{
						logger?.Warning($"Didn't add file {file} to index - already exists (how???)");
					}
					else
					{
						var elapsed = sw.Elapsed - startTime;
						_ = timePerFile.TryAdd(fileInfo.S5Header.Name, elapsed);
					}
				}
				catch (Exception ex)
				{
					logger?.Error($"Failed to load \"{file}\"", ex);

					var obj = SawyerStreamReader.LoadS5HeaderFromFile(file);
					var indexObjectHeader = new IndexObjectHeader(obj.Name, obj.ObjectType, obj.Checksum, null);
					_ = ccHeaderIndex.TryAdd(file, indexObjectHeader);
				}
				finally
				{
					_ = Interlocked.Increment(ref count);
					progress?.Report(count / (float)allFiles.Length);
				}
				//}
			});

			HeaderIndex = ccHeaderIndex.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			ObjectCache = ccObjectCache.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			sw.Stop();
			logger?.Info("Finished creating index");
			logger?.Debug($"Time time={sw.Elapsed}");

			if (timePerFile.IsEmpty)
			{
				_ = timePerFile.TryAdd("<no items>", TimeSpan.Zero);
			}

			var slowest = timePerFile.MaxBy(x => x.Value.Ticks);
			logger?.Debug($"Slowest file={slowest.Key} Time={slowest.Value}");

			var average = timePerFile.Average(x => x.Value.TotalMilliseconds);
			logger?.Debug($"Average time={average}ms");

			var median = timePerFile.OrderBy(x => x.Value).Skip(timePerFile.Count / 2).Take(1).Single();
			logger?.Debug($"Median time={median.Value}ms");
		}

		public void SaveFile(string path, UiLocoFile obj)
		{
			if (obj == null)
			{
				logger?.Error($"Cannot save an object with a null loco object - the file would be empty!");
				return;
			}

			SawyerStreamWriter.Save(path, obj.DatFileInfo.S5Header.Name, obj.LocoObject);
		}

		public bool LoadDataDirectory(string directory)
		{
			if (!Directory.Exists(directory))
			{
				logger?.Warning("Invalid directory: doesn't exist");
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

		public async Task LoadObjDirectoryAsync(string directory, IProgress<float>? progress, bool useExistingIndex)
		{
			await Task.Run(() => LoadObjDirectory(directory, progress, useExistingIndex));
			await Task.Run(() => SaveSettings());
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
			var allFiles = Directory.GetFiles(directory, "*.dat", SearchOption.AllDirectories);
			if (useExistingIndex && File.Exists(Settings.GetObjDataFullPath(Settings.IndexFileName)))
			{
				HeaderIndex = DeserialiseHeaderIndexFromFile(Settings.GetObjDataFullPath(Settings.IndexFileName)) ?? HeaderIndex;

				var a = HeaderIndex.Keys.Except(allFiles);
				var b = allFiles.Except(HeaderIndex.Keys);
				if (a.Any() || b.Any())
				{
					logger?.Warning("Selected directory had an index file but it was outdated; suggest recreating it when you have a moment");
					//logger?.Warning("Files in index that weren't in the directory:");
					//foreach (var aa in a)
					//{
					//	logger?.Warning($"  {aa}");
					//}

					//logger?.Warning("Files in directory that weren't in the index:");
					//foreach (var bb in b)
					//{
					//	logger?.Warning($"  {bb}");
					//}
				}
			}
			else
			{
				logger?.Info("Recreating index file");
				CreateIndex(allFiles, progress);
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

		public UiLocoFile? LoadAndCacheObject(string filename)
		{
			if (string.IsNullOrEmpty(filename) || !filename.EndsWith(".dat", StringComparison.InvariantCultureIgnoreCase) || !File.Exists(filename))
			{
				return null;
			}

			if (ObjectCache.TryGetValue(filename, out var value))
			{
				return value;
			}
			else
			{
				var obj = SawyerStreamReader.LoadFullObjectFromFile(filename, logger: logger);
				var uiObj = new UiLocoFile { DatFileInfo = obj.DatFileInfo, LocoObject = obj.LocoObject };
				_ = ObjectCache.TryAdd(filename, uiObj);
				return uiObj;
			}
		}
	}
}
