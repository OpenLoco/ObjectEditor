using System.IO;
using System;
using OpenLoco.ObjectEditor.Settings;
using System.Text.Json;
using OpenLoco.ObjectEditor.Logging;
using Zenith.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using DynamicData;
using AvaGui.ViewModels;
using System.Net.Http;

namespace AvaGui.Models
{
	public class Data
	{
		[JsonPropertyName("C00")] public string ObjectName { get; set; }
		[JsonPropertyName("C01")] public string Image { get; set; }
		[JsonPropertyName("C02")] public string DescriptionAndFile { get; set; }
		[JsonPropertyName("C03")] public string ClassNumber { get; set; }
		[JsonPropertyName("C04")] public string Type { get; set; }
		[JsonPropertyName("C05")] public string TrackType { get; set; }
		[JsonPropertyName("C06")] public string Designed { get; set; }
		[JsonPropertyName("C07")] public string Obsolete { get; set; }
		[JsonPropertyName("C08")] public string Speed { get; set; }
		[JsonPropertyName("C09")] public string Power { get; set; }
		[JsonPropertyName("C10")] public string Weight { get; set; }
		[JsonPropertyName("C11")] public string Reliability { get; set; }
		[JsonPropertyName("C12")] public string Length { get; set; }
		[JsonPropertyName("C13")] public string NumCompat { get; set; }
		[JsonPropertyName("C14")] public string Sprites { get; set; }
		[JsonPropertyName("C15")] public string CargoCapacity1 { get; set; }
		[JsonPropertyName("C16")] public string CargoType1 { get; set; }
		[JsonPropertyName("C17")] public string CargoCapacity2 { get; set; }
		[JsonPropertyName("C18")] public string CargoType2 { get; set; }
		[JsonPropertyName("C19")] public string Creator { get; set; }
		[JsonPropertyName("C20")] public string _Tags { get; set; }

		public string[] Tags => _Tags.Split(" ");
	}

	public class GlenDBSchema
	{
		public IList<Data> data { get; set; }
	}

	public class ObjectEditorModel
	{
		public EditorSettings Settings { get; private set; }

		public ILogger Logger;

		public HeaderIndex HeaderIndex { get; private set; } = [];

		public PaletteMap PaletteMap { get; set; }

		public G1Dat? G1 { get; set; }

		public Dictionary<string, byte[]> Music { get; } = [];

		public Dictionary<string, byte[]> MiscellaneousTracks { get; } = [];

		public Dictionary<string, byte[]> SoundEffects { get; } = [];

		public Dictionary<string, byte[]> Tutorials { get; } = [];

		public Dictionary<string, ObjectMetadata> Metadata { get; set; } = [];

		public Collection<string> MiscFiles { get; } = [];

		public const string ApplicationName = "OpenLoco Object Editor";

		public static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

		public static string SettingsFile => Path.Combine(SettingsPath, "settings.json");

		public ObservableCollection<LogLine> LoggerObservableLogs = [];

		public HttpClient WebClient { get; }

		public ObjectEditorModel()
		{
			Logger = new Logger();
			LoggerObservableLogs = [];
			Logger.LogAdded += (sender, laea) => LoggerObservableLogs.Insert(0, laea.Log);

			LoadSettings();
			LoadMetadata();

			// create http client
			WebClient = new HttpClient() { BaseAddress = new Uri("https://localhost:7230"), };
		}

		public void LoadSettings()
		{
			if (!File.Exists(SettingsFile))
			{
				Settings = new();
				SaveSettings();
				return;
			}

			var text = File.ReadAllText(SettingsFile);
			var settings = JsonSerializer.Deserialize<EditorSettings>(text);
			Verify.NotNull(settings);

			Settings = settings!;

			if (ValidateSettings(Settings, Logger))
			{
				if (File.Exists(IndexFilename))
				{
					Logger?.Info($"Loading header index from \"{IndexFilename}\"");
					LoadObjDirectoryAsync(Settings.ObjDataDirectory, new Progress<float>(), true).Wait();
				}
			}
		}

		public string IndexFilename
			=> Settings.GetObjDataFullPath(Settings.IndexFileName);

		public string MetadataFilename
			=> Settings.GetObjDataFullPath(Settings.MetadataFileName);

		public void LoadMetadata()
		{
			if (!File.Exists(MetadataFilename))
			{
				Logger?.Info($"Metadata file does not exist: \"{MetadataFilename}\"");
				Metadata = [];
				return;
			}

			Logger?.Info($"Loading metadata from \"{MetadataFilename}\"");

			var text = File.ReadAllText(MetadataFilename);
			var metadata = JsonSerializer.Deserialize<Dictionary<string, ObjectMetadata>>(text);
			Verify.NotNull(metadata);

			Metadata = metadata!;
		}

		public void SaveMetadata()
		{
			var options = GetJsonSerializationOptions();
			var text = JsonSerializer.Serialize(Metadata, options);

			var parentDir = Path.GetDirectoryName(MetadataFilename);
			if (parentDir != null && !Directory.Exists(parentDir))
			{
				_ = Directory.CreateDirectory(parentDir);
			}

			File.WriteAllText(MetadataFilename, text);
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
			var options = GetJsonSerializationOptions();
			var text = JsonSerializer.Serialize(Settings, options);

			var parentDir = Path.GetDirectoryName(SettingsFile);
			if (parentDir != null && !Directory.Exists(parentDir))
			{
				_ = Directory.CreateDirectory(parentDir);
			}

			File.WriteAllText(SettingsFile, text);
		}

		public ObjectMetadata LoadObjectMetadata(string objectName, uint checksum)
		{
			if (!Metadata.TryGetValue(objectName, out var value))
			{
				var text = File.ReadAllText(@"G:\\My Drive\\Locomotion\\Objects\\dataBase.json");
				var data = JsonSerializer.Deserialize<GlenDBSchema>(text); // this loads and deserialises the entire thing every time, rip
				var matching = data!.data.Where(x => x.ObjectName == objectName);
				var first = matching.FirstOrDefault();

				value = new ObjectMetadata(objectName, checksum);

				if (first != null)
				{
					value.Description = first.DescriptionAndFile;
					value.Author = first.Creator;
					value.Tags.AddRange(first.Tags);

				}
				Metadata.Add(objectName, value);
			}

			return value;
		}

		public bool TryLoadObject(FileSystemItemBase filesystemItem, out UiLocoFile? uiLocoFile)
		{
			if (string.IsNullOrEmpty(filesystemItem.Path))
			{
				uiLocoFile = null;
				return false;
			}

			DatFileInfo? fileInfo;
			ILocoObject? locoObject;

			try
			{
				if (filesystemItem.Path.Equals("<online>", StringComparison.OrdinalIgnoreCase))
				{
					using HttpResponseMessage response = Task.Run(async () => await WebClient.GetAsync($"/objects/originaldat/{filesystemItem.Name}")).Result;
					// wait for request to arrive back
					if (!response.IsSuccessStatusCode)
					{
						// failed
					}

					var base64obj = Task.Run(response.Content.ReadAsStringAsync).Result;
					var objdata = Convert.FromBase64String(base64obj);
					(fileInfo, locoObject) = SawyerStreamReader.LoadFullObjectFromStream(objdata, $"{filesystemItem.Path}/{filesystemItem.Name}", true, Logger);
				}
				else
				{
					(fileInfo, locoObject) = SawyerStreamReader.LoadFullObjectFromFile(filesystemItem.Path, logger: Logger);
				}
			}
			catch (Exception ex)
			{
				Logger?.Error($"Unable to load {filesystemItem.Path}", ex);
				uiLocoFile = null;
				return false;
			}

			if (locoObject == null || fileInfo == null)
			{
				Logger?.Error($"Unable to load {filesystemItem.Path}. FileInfo={fileInfo}");
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

			if (useExistingIndex && File.Exists(IndexFilename))
			{
				HeaderIndex = DeserialiseHeaderIndexFromFile(IndexFilename) ?? HeaderIndex;

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
				SerialiseHeaderIndexToFile(IndexFilename, HeaderIndex, GetJsonSerializationOptions());
			}

			SaveSettings();
		}

		private static JsonSerializerOptions GetJsonSerializationOptions()
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

			return JsonSerializer.Deserialize<HeaderIndex>(json, GetJsonSerializationOptions()) ?? [];
		}
	}
}
