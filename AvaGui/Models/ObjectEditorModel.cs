using System.IO;
using System;
using OpenLoco.Dat.Settings;
using System.Text.Json;
using OpenLoco.Dat.Logging;
using Zenith.Core;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Data;
using OpenLoco.Dat;
using System.Collections.ObjectModel;
using DynamicData;
using System.Net.Http;
using Avalonia.Threading;
using OpenLoco.Shared;
using System.Net.Http.Json;
using OpenLoco.Dat.Types;
using OpenLoco.Db.Schema;

namespace AvaGui.Models
{
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
			Logger.LogAdded += (sender, laea) => Dispatcher.UIThread.Post(() => LoggerObservableLogs.Insert(0, laea.Log));

			LoadSettings();
			//Metadata = Utils.LoadMetadata(MetadataFilename);
			Metadata = Utils.LoadMetadata("G:\\My Drive\\Locomotion\\Objects\\dataBase.json");

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

			if (ValidateSettings(Settings, Logger) && File.Exists(IndexFilename))
			{
				Logger?.Info($"Loading header index from \"{IndexFilename}\"");
				Task.Run(async () => await LoadObjDirectoryAsync(Settings.ObjDataDirectory, new Progress<float>(), true)).Wait();
			}
		}

		public string IndexFilename
			=> Settings.GetObjDataFullPath(Settings.IndexFileName);

		public string MetadataFilename
			=> Settings.GetObjDataFullPath(Settings.MetadataFileName);

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
			var text = JsonSerializer.Serialize(Settings);

			var parentDir = Path.GetDirectoryName(SettingsFile);
			if (parentDir != null && !Directory.Exists(parentDir))
			{
				_ = Directory.CreateDirectory(parentDir);
			}

			File.WriteAllText(SettingsFile, text);
		}

		public bool TryLoadObject(FileSystemItem filesystemItem, out UiLocoFile? uiLocoFile)
		{
			if (string.IsNullOrEmpty(filesystemItem.Path))
			{
				uiLocoFile = null;
				return false;
			}

			DatFileInfo? fileInfo = null;
			ILocoObject? locoObject = null;

			try
			{
				if (filesystemItem.FileLocation == FileLocation.Online)
				{
					using HttpResponseMessage response = Task.Run(async () => await WebClient.GetAsync($"/objects/originaldat/{filesystemItem.Path}")).Result;
					// wait for request to arrive back
					if (!response.IsSuccessStatusCode)
					{
						// failed
					}

					var locoObj = response.Content.ReadFromJsonAsync<TblLocoObject>().Result;

					if (locoObj == null || locoObj.OriginalBytes.Length == 0)
					{
						Logger?.Error($"Unable to load {filesystemItem.Path} from online");
					}
					else
					{
						(fileInfo, locoObject) = SawyerStreamReader.LoadFullObjectFromStream(locoObj.OriginalBytes, $"{filesystemItem.Path}/{filesystemItem.Name}", true, Logger);
					}
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

			HeaderIndex = index.ToDictionary(x => x.Filename, x => x);

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
			SaveSettings();
			var allFiles = SawyerStreamUtils.GetDatFilesInDirectory(directory).ToArray();

			if (useExistingIndex && File.Exists(IndexFilename))
			{
				HeaderIndex = ObjectIndexManager.DeserialiseHeaderIndexFromFile(IndexFilename, Logger) ?? HeaderIndex;

				if (HeaderIndex == null || HeaderIndex.Any(x => string.IsNullOrEmpty(x.Value.Filename) || string.IsNullOrEmpty(x.Value.ObjectName)))
				{
					Logger?.Warning("Index file appears to be malformed - recreating now.");
					await RecreateIndex(progress, allFiles);
					return;
				}

				if (HeaderIndex.Keys.Except(allFiles).Any() || allFiles.Except(HeaderIndex.Keys).Any())
				{
					Logger?.Warning("Index file appears to be outdated - recreating now.");
					await RecreateIndex(progress, allFiles);
					return;
				}
			}
			else
			{
				await RecreateIndex(progress, allFiles);
				return;
			}

			async Task RecreateIndex(IProgress<float> progress, string[] allFiles)
			{
				Logger?.Info("Recreating index file");
				await CreateIndex(allFiles, progress); // do we need the array?
				ObjectIndexManager.SerialiseHeaderIndexToFile(IndexFilename, HeaderIndex, Logger);
			}
		}
	}
}
