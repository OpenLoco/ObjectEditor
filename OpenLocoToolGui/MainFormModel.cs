global using HeaderIndex = System.Collections.Generic.Dictionary<string, OpenLocoToolGui.IndexObjectHeader>;
global using ObjectCache = System.Collections.Generic.Dictionary<string, OpenLocoToolGui.UiLocoObject>;

using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;
using OpenLocoTool.Headers;
using System.Diagnostics;

namespace OpenLocoToolGui
{
	class MainFormModel
	{
		private readonly ILogger logger;

		public HeaderIndex HeaderIndex { get; private set; } = [];

		public ObjectCache ObjectCache { get; private set; } = [];

		//public OpenLocoTool.ObjectManager ObjectManager { get; private set; } = new();

		public string PaletteFile
		{
			get => Settings.PaletteFile;
			set
			{
				Settings.PaletteFile = value;
				LoadPalette();
			}
		}

		private void LoadPalette()
		{
			//if (G1 == null)
			{
				try
				{
					var paletteBitmap = new Bitmap(Settings.PaletteFile);
					Palette = PaletteHelpers.PaletteFromBitmap(paletteBitmap);
					SaveSettings();
					logger.Debug($"Successfully loaded palette file {Settings.PaletteFile}");
				}
				catch (ArgumentException ex)
				{
					logger.Error(ex);
				}
			}
			//else
			//{
			//	var g1PaletteElement = G1.G1Elements[ImageIds.MainPalette];
			//	Palette = g1PaletteElement.ImageData
			//		.Take(256 * 3)
			//		.Chunk(3)
			//		.Select(x => Color.FromArgb(x[2], x[1], x[0]))
			//		.ToArray();
			//}
		}

		public Color[] Palette { get; private set; }

		public G1Dat G1 { get; set; }

		public MainFormModel(ILogger logger, string settingsFile)
		{
			this.logger = logger;

			LoadSettings(settingsFile);

			// Load all cargo objects on startup
			// Until a better solution is found (dynamic load-on-demand) we'll just do this
			// for now. We'll have to do this for every dependent object type

			var dependentObjectTypes = new HashSet<ObjectType>() { ObjectType.Cargo };
			foreach (var depObjectType in dependentObjectTypes)
			{
				logger.Debug($"Preloading dependent {depObjectType} objects");
			}

			foreach (var dep in HeaderIndex.Where(kvp => dependentObjectTypes.Contains(kvp.Value.ObjectType)))
			{
				//try
				//{
				SawyerStreamReader.LoadFull(dep.Key);
				//}
				//catch (Exception ex)
				//{
				//	logger.Error($"File=\"{dep}\" Message=\"{ex.Message}\"");
				//}
			}
		}

		public GuiSettings Settings { get; private set; }

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
			Settings = JsonSerializer.Deserialize<GuiSettings>(text);

			if (!ValidateSettings(Settings, logger))
			{
				return;
			}

			if (File.Exists(Settings.IndexFilePath))
			{
				logger.Info($"Loading header index from \"{Settings.IndexFileName}\"");
				LoadObjDirectory(Settings.ObjDataDirectory, new Progress<float>(), true);
			}

			LoadPalette();
		}

		static bool ValidateSettings(GuiSettings settings, ILogger logger)
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
			File.WriteAllText(SettingsFile, text);
		}

		// this method loads every single object entirely. it takes a long time to run
		void CreateIndex(string[] allFiles, IProgress<float> progress)
		{
			ConcurrentDictionary<string, IndexObjectHeader> ccHeaderIndex = new(); // key is full path/filename
			ConcurrentDictionary<string, UiLocoObject> ccObjectCache = new(); // key is full path/filename

			var count = 0;

			logger.Info($"Creating index on {allFiles.Length} files");
			var sw = new Stopwatch();
			sw.Start();

			Parallel.ForEach(allFiles, (file) =>
			//foreach (var file in allFiles)
			{
				try
				{
					var (fileInfo, locoObject) = SawyerStreamReader.LoadFull(file);
					if (!ccObjectCache.TryAdd(file, new UiLocoObject { DatFileInfo = fileInfo, LocoObject = locoObject }))
					{
						logger.Warning($"Didn't add file {file} to cache - already exists (how???)");
					}

					VehicleType? veh = null;
					if (locoObject.Object is VehicleObject vo)
					{
						veh = vo.Type;
					}

					var indexObjectHeader = new IndexObjectHeader(fileInfo.S5Header.Name, fileInfo.S5Header.ObjectType, veh);
					if (!ccHeaderIndex.TryAdd(file, indexObjectHeader))
					{
						logger.Warning($"Didn't add file {file} to index - already exists (how???)");
					}
				}
				catch (Exception ex)
				{
					logger.Error($"Failed to load \"{file}\"", ex);

					var obj = SawyerStreamReader.LoadHeader(file);
					var indexObjectHeader = new IndexObjectHeader(obj.Name, obj.ObjectType, null);
					ccHeaderIndex.TryAdd(file, indexObjectHeader);
				}
				finally
				{
					Interlocked.Increment(ref count);
					progress.Report(count / (float)allFiles.Length);
				}
				//}
			});

			HeaderIndex = ccHeaderIndex.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			ObjectCache = ccObjectCache.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			sw.Stop();
			logger.Info($"Finished creating index. Time={sw.Elapsed}");
		}

		public static void SaveFile(string path, UiLocoObject obj)
			=> SawyerStreamWriter.Save(path, obj.DatFileInfo.S5Header.Name, obj.LocoObject);

		public bool LoadDataDirectory(string directory)
		{
			if (!Directory.Exists(directory))
			{
				logger.Warning("Invalid directory");
				return false;
			}

			Settings.DataDirectory = directory;

			// load G1 only for now
			G1 = SawyerStreamReader.LoadG1(Settings.G1Path);
			LoadPalette(); // update palette from g1

			SaveSettings();

			return true;
		}

		public void LoadObjDirectory(string directory, IProgress<float> progress, bool useExistingIndex)
		{
			if (!Directory.Exists(directory))
			{
				return;
			}

			Settings.ObjDataDirectory = directory;
			var allFiles = Directory.GetFiles(directory, "*.dat", SearchOption.AllDirectories);
			if (useExistingIndex && File.Exists(Settings.IndexFilePath))
			{
				HeaderIndex = DeserialiseHeaderIndexFromFile(Settings.IndexFilePath) ?? HeaderIndex;

				var a = HeaderIndex.Keys.Except(allFiles);
				var b = allFiles.Except(HeaderIndex.Keys);
				if (a.Any() || b.Any())
				{
					logger.Warning("Selected directory had an index file but it was outdated; suggest recreating it when you have a moment");
					logger.Warning("Files in index that weren't in the directory:");
					foreach (var aa in a)
					{
						logger.Warning($"  {aa}");
					}

					logger.Warning("Files in directory that weren't in the index:");
					foreach (var bb in b)
					{
						logger.Warning($"  {bb}");
					}
				}
			}
			else
			{
				logger.Info("Recreating index file");
				CreateIndex(allFiles, progress);
				SerialiseHeaderIndexToFile(Settings.IndexFilePath, HeaderIndex, GetOptions());
			}

			SaveSettings();
		}

		private static JsonSerializerOptions GetOptions()
			=> new() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, };

		static void SerialiseHeaderIndexToFile(string filename, HeaderIndex headerIndex, JsonSerializerOptions options)
		{
			var json = JsonSerializer.Serialize(headerIndex, options);
			File.WriteAllText(filename, json);
		}

		static HeaderIndex? DeserialiseHeaderIndexFromFile(string filename)
		{
			if (!File.Exists(filename))
			{
				return null;
			}

			var json = File.ReadAllText(filename);

			return JsonSerializer.Deserialize<HeaderIndex>(json, GetOptions()) ?? [];
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
				var obj = SawyerStreamReader.LoadFull(filename);
				var uiObj = new UiLocoObject { DatFileInfo = obj.Item1, LocoObject = obj.Item2 };
				ObjectCache.TryAdd(filename, uiObj);
				return uiObj;
			}
		}
	}
}
