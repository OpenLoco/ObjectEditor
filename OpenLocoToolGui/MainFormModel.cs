global using HeaderIndex = System.Collections.Generic.Dictionary<string, OpenLocoToolGui.IndexObjectHeader>;
global using ObjectCache = System.Collections.Generic.Dictionary<string, OpenLocoTool.DatFileParsing.ILocoObject>;

using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Objects;
using OpenLocoToolCommon;

namespace OpenLocoToolGui
{
	class MainFormModel
	{
		private readonly ILogger logger;
		private readonly SawyerStreamReader reader;
		private readonly SawyerStreamWriter writer;

		public HeaderIndex HeaderIndex { get; private set; } = new();

		public ObjectCache ObjectCache { get; private set; } = new();

		public string PaletteFile
		{
			get => Settings.PaletteFile;
			set
			{
				Settings.PaletteFile = value;
				LoadPaletteFile();
			}
		}

		private void LoadPaletteFile()
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

		public Color[] Palette { get; private set; }

		public IG1Dat G1 { get; set; }

		public MainFormModel(ILogger logger, string settingsFile)
		{
			this.logger = logger;
			reader = new SawyerStreamReader(logger);
			writer = new SawyerStreamWriter(logger);

			LoadSettings(settingsFile);
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
				LoadDirectory(Settings.ObjDataDirectory, new Progress<float>(), true);
			}

			LoadPaletteFile();
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
			var text = JsonSerializer.Serialize(Settings, new JsonSerializerOptions() { WriteIndented = true });
			File.WriteAllText(SettingsFile, text);
		}

		// this method loads every single object entirely. it takes a long time to run
		void CreateIndex(string[] allFiles, IProgress<float> progress)
		{
			ConcurrentDictionary<string, IndexObjectHeader> ccHeaderIndex = new(); // key is full path/filename
			ConcurrentDictionary<string, ILocoObject> ccObjectCache = new(); // key is full path/filename

			var total = (float)allFiles.Length;
			var count = 0;
			Parallel.ForEach(allFiles, (file) =>
			{
				try
				{
					var locoObject = reader.LoadFull(file);
					if (!ccObjectCache.TryAdd(file, locoObject))
					{
						logger.Warning($"Didn't add file {file} to cache - already exists (how???)");
					}

					VehicleType? veh = null;
					if (locoObject.Object is VehicleObject vo)
						veh = vo.Type;

					var indexObjectHeader = new IndexObjectHeader(locoObject.S5Header.Name, locoObject.S5Header.ObjectType, veh);
					if (!ccHeaderIndex.TryAdd(file, indexObjectHeader))
					{
						logger.Warning($"Didn't add file {file} to index - already exists (how???)");
					}
				}
				catch (Exception ex)
				{
					logger.Error(ex);
				}
				finally
				{
					Interlocked.Increment(ref count);
					progress.Report(count / total);
				}
			});

			HeaderIndex = ccHeaderIndex.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			ObjectCache = ccObjectCache.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		public void SaveFile(string path, ILocoObject obj)
		{
			writer.Save(path, obj);
		}

		public bool LoadDataDirectory(string directory)
		{
			if (!Directory.Exists(directory))
			{
				logger.Warning("Invalid directory");
				return false;
			}
			Settings.DataDirectory = directory;

			// load G1 only for now
			G1 = reader.LoadG1(Settings.G1Path);

			SaveSettings();

			return true;
		}

		public void LoadDirectory(string directory, IProgress<float> progress, bool useExistingIndex)
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
				SerialiseHeaderIndexToFile(Settings.IndexFilePath, HeaderIndex);
			}

			SaveSettings();
		}

		static void SerialiseHeaderIndexToFile(string filename, HeaderIndex headerIndex)
		{
			var json = JsonSerializer.Serialize(headerIndex, new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, });
			File.WriteAllText(filename, json);
		}

		static HeaderIndex? DeserialiseHeaderIndexFromFile(string filename)
		{
			if (!File.Exists(filename))
			{
				return null;
			}

			var json = File.ReadAllText(filename);
			return JsonSerializer.Deserialize<HeaderIndex>(json, new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, }) ?? new();
		}

		public ILocoObject? LoadAndCacheObject(string filename)
		{
			if (string.IsNullOrEmpty(filename) || !filename.EndsWith(".dat", StringComparison.InvariantCultureIgnoreCase) || !File.Exists(filename))
			{
				return null;
			}

			if (ObjectCache.ContainsKey(filename))
			{
				return ObjectCache[filename];
			}
			else
			{
				var obj = reader.LoadFull(filename);
				ObjectCache.TryAdd(filename, obj);
				return obj;
			}
		}
	}
}
