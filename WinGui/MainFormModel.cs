global using ObjectCache = System.Collections.Generic.Dictionary<string, OpenLoco.WinGui.UiLocoObject>;
using Dat;
using OpenLoco.Common;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Zenith.Core;

namespace OpenLoco.WinGui
{
	class MainFormModel
	{
		private readonly ILogger logger;

		public ObjectIndex ObjectIndex { get; private set; }

		public ObjectCache ObjectCache { get; private set; } = [];

		//public OpenLocoObjectEditor.ObjectManager ObjectManager { get; private set; } = new();

		public PaletteMap PaletteMap { get; set; }

		public G1Dat? G1 { get; set; }

		public Dictionary<string, byte[]> Music { get; set; } = [];

		public Dictionary<string, byte[]> MiscellaneousTracks { get; set; } = [];

		public Dictionary<string, byte[]> SoundEffects { get; set; } = [];

		public Dictionary<string, byte[]> Tutorials { get; set; } = [];

		public List<string> MiscFiles { get; set; } = [];

		public MainFormModel(ILogger logger, string settingsFile, PaletteMap paletteMap)
		{
			this.logger = logger;
			PaletteMap = paletteMap;

			LoadSettings(settingsFile);

			// Load all cargo objects on startup
			// Until a better solution is found (dynamic load-on-demand) we'll just do this
			// for now. We'll have to do this for every dependent object type

			var dependentObjectTypes = new HashSet<ObjectType>() { ObjectType.Cargo };
			foreach (var depObjectType in dependentObjectTypes)
			{
				logger.Debug($"Preloading dependent {depObjectType} objects");
			}

			foreach (var dep in ObjectIndex.Objects.Where(x => x is ObjectIndexEntry oi && dependentObjectTypes.Contains(oi.ObjectType)))
			{
				var filename = Path.Combine(Settings.ObjDataDirectory, dep.Filename);
#if DEBUG
				SawyerStreamReader.LoadFullObjectFromFile(filename, logger);
#else
				try
				{
					SawyerStreamReader.LoadFullObjectFromFile(filename);
				}
				catch (Exception ex)
				{
					logger.Error($"File=\"{filename}\" Message=\"{ex.Message}\"");
				}
#endif
			}
		}

		public EditorSettings Settings { get; private set; }

		public string SettingsFile { get; set; }

		public void LoadSettings(string settingsFile)
		{
			SettingsFile = settingsFile;

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
				logger.Info($"Loading header index from \"{Settings.IndexFileName}\"");
				LoadObjDirectory(Settings.ObjDataDirectory, new Progress<float>(), true);
			}
		}

		static bool ValidateSettings(EditorSettings settings, ILogger logger)
		{
			if (settings == null)
			{
				logger.Error("Invalid settings file: Unable to deserialise settings file");
				return false;
			}

			if (string.IsNullOrEmpty(settings.ObjDataDirectory))
			{
				logger.Warning("Invalid settings file: Object directory was null or empty");
				return false;
			}

			if (!Directory.Exists(settings.ObjDataDirectory))
			{
				logger.Warning($"Invalid settings file: Directory \"{settings.ObjDataDirectory}\" does not exist");
				return false;
			}

			return true;
		}

		public void SaveSettings()
		{
			var options = GetOptions();
			var text = JsonSerializer.Serialize(Settings, options);

			var parentDir = Path.GetDirectoryName(SettingsFile);
			if (parentDir != null && !Directory.Exists(parentDir))
			{
				_ = Directory.CreateDirectory(parentDir);
			}

			File.WriteAllText(SettingsFile, text);
		}

		// this method loads every single object entirely. it takes a long time to run
		void CreateIndex(string[] allFiles, IProgress<float>? progress)
		{
			ConcurrentDictionary<string, ObjectIndexEntryBase> ccHeaderIndex = new(); // key is full path/filename
			ConcurrentDictionary<string, UiLocoObject> ccObjectCache = new(); // key is full path/filename

			var count = 0;

			ConcurrentDictionary<string, TimeSpan> timePerFile = new();

			logger.Info($"Creating index on {allFiles.Length} files");
			var sw = new Stopwatch();
			sw.Start();

			_ = Parallel.ForEach(allFiles, new ParallelOptions() { MaxDegreeOfParallelism = 100 }, (file) =>
			//foreach (var file in allFiles)
			{
				try
				{
					var startTime = sw.Elapsed;
					var obj = SawyerStreamReader.LoadFullObjectFromFile(file, logger);

					if (obj == null || obj.Value.LocoObject == null)
					{
						logger.Error($"Unable to load {file}. FileInfo={obj.Value.DatFileInfo}");
						return;
					}

					var fileInfo = obj.Value.DatFileInfo;
					var locoObject = obj.Value.LocoObject;

					if (!ccObjectCache.TryAdd(file, new UiLocoObject(fileInfo, locoObject)))
					{
						logger.Warning($"Didn't add file {file} to cache - already exists (how???)");
					}

					VehicleType? veh = null;
					if (locoObject.Object is VehicleObject vo)
					{
						veh = vo.Type;
					}

					var s5 = fileInfo.S5Header;
					var isVanilla = s5.IsOriginal();
					var indexObjectHeader = new ObjectIndexEntry(file, s5.Name, s5.ObjectType, isVanilla, s5.Checksum, veh);
					if (!ccHeaderIndex.TryAdd(file, indexObjectHeader))
					{
						logger.Warning($"Didn't add file {file} to index - already exists (how???)");
					}
					else
					{
						var elapsed = sw.Elapsed - startTime;
						_ = timePerFile.TryAdd(fileInfo.S5Header.Name, elapsed);
					}
				}
				catch (Exception ex)
				{
					logger.Error($"Failed to load \"{file}\"", ex);
					_ = ccHeaderIndex.TryAdd(file, new ObjectIndexFailedEntry(file));
				}
				finally
				{
					_ = Interlocked.Increment(ref count);
					progress?.Report(count / (float)allFiles.Length);
				}
				//}
			});

			ObjectIndex = new ObjectIndex() { Objects = ccHeaderIndex.Values.OfType<ObjectIndexEntry>().ToList(), ObjectsFailed = ccHeaderIndex.Values.OfType<ObjectIndexFailedEntry>().ToList() };
			ObjectCache = ccObjectCache.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			sw.Stop();
			logger.Info("Finished creating index");

			logger.Debug($"Time time={sw.Elapsed}");

			if (timePerFile.IsEmpty)
			{
				_ = timePerFile.TryAdd("<no items>", TimeSpan.Zero);
			}

			var slowest = timePerFile.MaxBy(x => x.Value.Ticks);
			logger.Debug($"Slowest file={slowest.Key} Time={slowest.Value}");

			var average = timePerFile.Average(x => x.Value.TotalMilliseconds);
			logger.Debug($"Average time={average}ms");

			var median = timePerFile.OrderBy(x => x.Value).Skip(timePerFile.Count / 2).Take(1).Single();
			logger.Debug($"Median time={median.Value}ms");
		}

		public void SaveFile(string path, UiLocoObject obj)
		{
			if (obj.LocoObject == null)
			{
				logger.Error($"Cannot save an object with a null loco object - the file would be empty!");
				return;
			}

			SawyerStreamWriter.Save(path, obj.DatFileInfo.S5Header.Name, obj.LocoObject);
		}

		public void LoadObjDirectory(string directory, IProgress<float>? progress, bool useExistingIndex)
		{
			if (!Directory.Exists(directory))
			{
				return;
			}

			Settings.ObjDataDirectory = directory;
			var allFiles = Directory.GetFiles(directory, "*.dat", SearchOption.AllDirectories);
			var indexFileName = Settings.GetObjDataFullPath(Settings.IndexFileName);
			if (useExistingIndex && File.Exists(indexFileName))
			{
				ObjectIndex = ObjectIndex.LoadIndex(indexFileName) ?? ObjectIndex;

				var filenames = ObjectIndex.Objects.Select(x => x.Filename).Concat(ObjectIndex.ObjectsFailed.Select(x => x.Filename));
				var a = filenames.Except(allFiles);
				var b = allFiles.Except(filenames);
				if (a.Any() || b.Any())
				{
					logger.Warning("Selected directory had an index file but it was outdated; suggest recreating it when you have a moment");
					//logger.Warning("Files in index that weren't in the directory:");
					//foreach (var aa in a)
					//{
					//	logger.Warning($"  {aa}");
					//}

					//logger.Warning("Files in directory that weren't in the index:");
					//foreach (var bb in b)
					//{
					//	logger.Warning($"  {bb}");
					//}
				}
			}
			else
			{
				logger.Info("Recreating index file");
				CreateIndex(allFiles, progress);
				SerialiseHeaderIndexToFile(Settings.GetObjDataFullPath(Settings.IndexFileName), ObjectIndex, GetOptions());
			}

			SaveSettings();
		}

		public bool LoadDataDirectory(string directory)
		{
			if (!Directory.Exists(directory))
			{
				logger.Warning("Invalid directory: doesn't exist");
				return false;
			}

			Settings.DataDirectory = directory;

			var allDataFiles = Directory.GetFiles(Settings.DataDirectory).Select(f => Path.GetFileName(f).ToLower()).ToHashSet();

			void LoadKnownData(HashSet<string> allFilesInDir, HashSet<string> knownFilenames, Dictionary<string, byte[]> dict)
			{
				dict.Clear();
				foreach (var music in knownFilenames.Select(f => f.ToLower()))
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
			G1 = SawyerStreamReader.LoadG1(Settings.GetDataFullPath(Settings.G1DatFileName), logger);

			//LoadPalette(); // update palette from g1

			SaveSettings();

			return true;
		}

		public bool LoadSCV5Directory(string directory)
		{
			if (!Directory.Exists(directory))
			{
				logger.Warning("Invalid directory: doesn't exist");
				return false;
			}

			Settings.SCV5Directory = directory;
			SaveSettings();


			return true;
		}

		private static JsonSerializerOptions GetOptions()
			=> new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, };

		static void SerialiseHeaderIndexToFile(string filename, ObjectIndex headerIndex, JsonSerializerOptions options, ILogger? logger = null)
		{
			logger?.Info($"Saved settings to {filename}");
			headerIndex.SaveIndex(filename, options);
		}

		public UiLocoObject? LoadAndCacheObject(string filename)
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
				var uiObj = new UiLocoObject(obj?.DatFileInfo, obj?.LocoObject);
				_ = ObjectCache.TryAdd(filename, uiObj);
				return uiObj;
			}
		}
	}
}
