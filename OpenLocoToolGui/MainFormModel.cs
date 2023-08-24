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
		private ILogger logger;
		private SawyerStreamReader reader;
		private SawyerStreamWriter writer;

		public HeaderIndex HeaderIndex { get; private set; } = new();

		public ObjectCache ObjectCache { get; private set; } = new();

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

			ValidateSettings(Settings, logger);

			if (File.Exists(Settings.IndexFilePath))
			{
				logger.Info($"Loading header index from \"{Settings.IndexFileName}\"");
				DeserialiseHeaderIndexFromFile();
			}
		}

		static void ValidateSettings(GuiSettings settings, ILogger logger)
		{
			if (settings == null)
			{
				logger.Error($"Unable to load settings");
				return;
			}

			if (string.IsNullOrEmpty(settings.ObjectDirectory))
			{
				logger.Warning("Object directory was null or empty");
				return;
			}

			if (!Directory.Exists(settings.ObjectDirectory))
			{
				logger.Warning($"Directory \"{settings.ObjectDirectory}\" does not exist");
				return;
			}
		}

		public void SaveSettings()
		{
			var text = JsonSerializer.Serialize(Settings, new JsonSerializerOptions() { WriteIndented = true });
			File.WriteAllText(SettingsFile, text);
		}

		public void CreateIndex(string[] allFiles, IProgress<float> progress)
		{
			CreateIndexCore(allFiles, progress);
			SerialiseHeaderIndexToFile();
		}

		// this method loads every single object entirely. it takes a long time to run
		void CreateIndexCore(string[] allFiles, IProgress<float> progress)
		{
			ConcurrentDictionary<string, IndexObjectHeader> ccHeaderIndex = new(); // key is full path/filename
			ConcurrentDictionary<string, ILocoObject> ccObjectCache = new(); // key is full path/filename

			var total = (float)allFiles.Length;
			var count = 0;
			Parallel.ForEach(allFiles, (file) =>
			{
				var locoObject = reader.LoadFull(file);
				if (!ccObjectCache.TryAdd(file, locoObject))
				{
					logger.Warning($"Didn't add file {file} to cache - already exists (how???)");
				}

				VehicleType? veh = null;
				if (locoObject.Object is VehicleObject vo)
					veh = vo.Type;

				var indexObjectHeader = new IndexObjectHeader(locoObject.ObjectHeader.Name, locoObject.ObjectHeader.ObjectType, veh);
				if (!ccHeaderIndex.TryAdd(file, indexObjectHeader))
				{
					logger.Warning($"Didn't add file {file} to index - already exists (how???)");
				}

				Interlocked.Increment(ref count);
				progress.Report(count / total);
			});

			HeaderIndex = ccHeaderIndex.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
			ObjectCache = ccObjectCache.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
		}

		public void LoadDirectory(string directory, IProgress<float> progress)
		{
			if (!Directory.Exists(directory))
			{
				return;
			}

			Settings.ObjectDirectory = directory;
			if (File.Exists(Settings.IndexFilePath))
			{
				DeserialiseHeaderIndexFromFile();
			}
			else
			{
				var allFiles = Directory.GetFiles(directory, "*.dat", SearchOption.AllDirectories);
				CreateIndex(allFiles, progress);
			}

			SaveSettings();
		}

		void SerialiseHeaderIndexToFile()
		{
			var json = JsonSerializer.Serialize(HeaderIndex, new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, });
			File.WriteAllText(Path.Combine(Settings.ObjectDirectory, Settings.IndexFilePath), json);
		}

		void DeserialiseHeaderIndexFromFile()
		{
			if (!File.Exists(Settings.IndexFilePath))
			{
				return;
			}

			var json = File.ReadAllText(Settings.IndexFilePath);
			HeaderIndex = JsonSerializer.Deserialize<HeaderIndex>(json, new JsonSerializerOptions() { WriteIndented = true, Converters = { new JsonStringEnumConverter() }, }) ?? new();
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
