using Avalonia.Threading;
using Dat;
using DynamicData;
using OpenLoco.Common;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using OpenLoco.Definitions.DTO;
using OpenLoco.Definitions.Web;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Zenith.Core;

namespace AvaGui.Models
{
	public class ObjectEditorModel
	{
		public EditorSettings Settings { get; private set; }

		public ILogger Logger;

		public ObjectIndex ObjectIndex { get; private set; }

		public ObjectIndex ObjectIndexOnline { get; set; }

		public Dictionary<int, DtoLocoObject> OnlineCache { get; set; } = [];

		public PaletteMap PaletteMap { get; set; }

		public G1Dat? G1 { get; set; }

		public Dictionary<string, byte[]> Music { get; } = [];

		public Dictionary<string, byte[]> MiscellaneousTracks { get; } = [];

		public Dictionary<string, byte[]> SoundEffects { get; } = [];

		public Dictionary<string, byte[]> Tutorials { get; } = [];

		public Collection<string> MiscFiles { get; } = [];

		public const string ApplicationName = "OpenLoco Object Editor";

		public static string SettingsPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);

#if DEBUG
		public static string SettingsFile => Path.Combine(SettingsPath, "settings-dev.json");
#else
		public static string SettingsFile => Path.Combine(SettingsPath, "settings.json");
#endif

		public ObservableCollection<LogLine> LoggerObservableLogs = [];

		public HttpClient WebClient { get; }

		public ObjectEditorModel()
		{
			Logger = new Logger();
			LoggerObservableLogs = [];
			Logger.LogAdded += (sender, laea) => Dispatcher.UIThread.Post(() => LoggerObservableLogs.Insert(0, laea.Log));

			LoadSettings();

			var server = Settings.UseHttps ? Settings.ServerAddressHttps : Settings.ServerAddressHttp;
			WebClient = new HttpClient() { BaseAddress = new Uri(server), };
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
			var settings = JsonSerializer.Deserialize<EditorSettings>(text, options: new() { WriteIndented = true });
			Verify.NotNull(settings);

			Settings = settings!;
			InitialiseDownloadDirectory();

			if (!ValidateSettings(Settings, Logger) && File.Exists(IndexFilename))
			{
				Logger.Error("Unable to validate settings file - please delete it and it will be recreated on next editor start-up.");
			}
		}

		void InitialiseDownloadDirectory()
		{
			if (string.IsNullOrEmpty(Settings.DownloadFolder))
			{
				Settings.DownloadFolder = Path.Combine(SettingsPath, "downloads");
			}

			if (!Directory.Exists(Settings.DownloadFolder))
			{
				_ = Directory.CreateDirectory(Settings.DownloadFolder);
			}
		}

		public string IndexFilename
			=> Settings.GetObjDataFullPath(Settings.IndexFileName);

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
			var text = JsonSerializer.Serialize(Settings, options: new() { WriteIndented = true });

			var parentDir = Path.GetDirectoryName(SettingsFile);
			if (parentDir != null && !Directory.Exists(parentDir))
			{
				_ = Directory.CreateDirectory(parentDir);
			}

			try
			{
				File.WriteAllText(SettingsFile, text);
			}
			catch (Exception ex)
			{
				Logger.Error(ex);
			}
		}

		public bool TryLoadObject(FileSystemItem filesystemItem, out UiLocoFile? uiLocoFile)
		{
			if (string.IsNullOrEmpty(filesystemItem.Filename))
			{
				uiLocoFile = null;
				return false;
			}

			DatFileInfo? fileInfo = null;
			ILocoObject? locoObject = null;
			MetadataModel? metadata = null;
			uiLocoFile = null;
			List<Image<Rgba32>?> images = [];

			try
			{
				if (filesystemItem.FileLocation == FileLocation.Online)
				{
					var uniqueObjectId = int.Parse(filesystemItem.Filename);

					if (!OnlineCache.TryGetValue(uniqueObjectId, out var locoObj))
					{
						Logger.Debug($"Didn't find object {filesystemItem.Name} with unique id {uniqueObjectId} in cache - downloading it");
						locoObj = Task.Run(async () => await Client.GetObjectAsync(WebClient, uniqueObjectId, true)).Result;

						if (locoObj == null)
						{
							Logger.Error($"Unable to object {filesystemItem.Name} with unique id {uniqueObjectId} from online - received no data");
							return false;
						}
						else if (locoObj.IsVanilla)
						{
							Logger.Info($"Unable to object {filesystemItem.Name} with unique id {uniqueObjectId} from online - requested object is a vanilla object and it is illegal to distribute copyright material");
							return false;
						}
						else if (locoObj.OriginalBytes == null || locoObj.OriginalBytes.Length == 0)
						{
							Logger.Error($"Unable to load object {filesystemItem.Name} with unique id {uniqueObjectId} from online - received no object data");
							return false;
						}

						Logger.Error($"Added object {filesystemItem.Name} with unique id {uniqueObjectId} to local cache");
						OnlineCache.Add(uniqueObjectId, locoObj);
					}
					else
					{
						Logger.Debug($"Found object {filesystemItem.Name} with unique id {uniqueObjectId} in cache - reusing it");
					}

					if (locoObj.OriginalBytes == null)
					{
						Logger.Error($"Received no data for {filesystemItem.Name}");
						return false;
					}
					var obj = SawyerStreamReader.LoadFullObjectFromStream(Convert.FromBase64String(locoObj.OriginalBytes), $"{filesystemItem.Filename}-{filesystemItem.Name}", true, Logger);
					if (obj == null)
					{
						Logger.Error($"Unable to load {filesystemItem.Name} from the received data");
						return false;
					}

					fileInfo = obj.Value.DatFileInfo;
					locoObject = obj.Value.LocoObject;
					metadata = new MetadataModel(locoObj.OriginalName, locoObj.OriginalChecksum)
					{
						Description = locoObj.Description,
						Authors = locoObj.Authors,
						CreationDate = locoObj.CreationDate,
						LastEditDate = locoObj.LastEditDate,
						UploadDate = locoObj.UploadDate,
						Tags = locoObj.Tags,
						Modpacks = locoObj.Modpacks,
						Availability = locoObj.Availability,
						Licence = locoObj.Licence,
					};

					foreach (var i in locoObject?.G1Elements)
					{
						images.Add(PaletteMap.ConvertG1ToRgb32Bitmap(i));
					}
				}
				else
				{
					var filename = Path.Combine(Settings.ObjDataDirectory, filesystemItem.Filename);
					var obj = SawyerStreamReader.LoadFullObjectFromFile(filename, logger: Logger);
					if (obj != null)
					{
						fileInfo = obj.Value.DatFileInfo;
						locoObject = obj.Value.LocoObject;
						metadata = null; // todo: look this up from internet anyways

						foreach (var i in locoObject?.G1Elements)
						{
							images.Add(PaletteMap.ConvertG1ToRgb32Bitmap(i));
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error($"Unable to load {filesystemItem.Filename}", ex);
				uiLocoFile = null;
				return false;
			}

			if (locoObject == null || fileInfo == null)
			{
				Logger.Error($"Unable to load {filesystemItem.Filename}");
				uiLocoFile = null;
				return false;
			}

			uiLocoFile = new UiLocoFile() { DatFileInfo = fileInfo, LocoObject = locoObject, Metadata = metadata, Images = images };
			return true;
		}

		public bool LoadDataDirectory(string directory)
		{
			if (!Directory.Exists(directory))
			{
				Logger.Warning("Invalid directory: doesn't exist");
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

		static Task? indexerTask;
		static readonly SemaphoreSlim taskLock = new(1, 1);

		public async Task LoadObjDirectoryAsync(string directory, IProgress<float> progress, bool useExistingIndex)
		{
			await taskLock.WaitAsync();

			try
			{
				if (indexerTask?.IsCompleted != false)
				{
					indexerTask = Task.Run(async () => await LoadObjDirectoryAsyncCore(directory, progress, useExistingIndex));
				}
			}
			finally
			{
				_ = taskLock.Release();
			}

			await indexerTask;
		}

		async Task LoadObjDirectoryAsyncCore(string directory, IProgress<float> progress, bool useExistingIndex)
		{
			if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory) || progress == null)
			{
				Logger.Error($"Couldn't start loading obj dir: {directory}");
				return;
			}

			Settings.ObjDataDirectory = directory;
			SaveSettings();
			var allFiles = SawyerStreamUtils.GetDatFilesInDirectory(directory).ToArray();

			if (useExistingIndex && File.Exists(IndexFilename))
			{
				var exception = false;

				try
				{
					ObjectIndex = await ObjectIndex.LoadIndexAsync(IndexFilename) ?? ObjectIndex;
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
					exception = true;
				}

				if (exception || ObjectIndex?.Objects == null || ObjectIndex.Objects.Any(x => string.IsNullOrEmpty(x.Filename) || (x is ObjectIndexEntry xx && string.IsNullOrEmpty(xx.ObjectName))) != false)
				{
					Logger.Warning("Index file format has changed or otherwise appears to be malformed - recreating now.");
					await RecreateIndex(directory, allFiles, progress);
					return;
				}

				var objectIndexFilenames = ObjectIndex.Objects.Select(x => x.Filename).Concat(ObjectIndex.ObjectsFailed.Select(x => x.Filename));
				if (objectIndexFilenames.Except(allFiles).Any() || allFiles.Except(objectIndexFilenames).Any())
				{
					Logger.Warning("Index file appears to be outdated - recreating now.");
					await RecreateIndex(directory, allFiles, progress);
				}
			}
			else
			{
				await RecreateIndex(directory, allFiles, progress);
			}

			async Task RecreateIndex(string rootObjectDirectory, string[] allFiles, IProgress<float> progress)
			{
				Logger.Info("Recreating index file");
				ObjectIndex = await ObjectIndex.CreateIndexAsync(rootObjectDirectory, allFiles, progress);
				ObjectIndex?.SaveIndex(IndexFilename);
			}
		}

		public async Task CheckForDatFilesNotOnServer()
		{
			if (ObjectIndex == null || ObjectIndexOnline == null || ObjectIndexOnline.Objects.Count == 0)
			{
				return;
			}

			Logger.Debug("Comparing local objects to object repository");

			var localButNotOnline = ObjectIndex.Objects.ExceptBy(ObjectIndexOnline.Objects.Select(
				x => (x.ObjectName, x.Checksum)),
				x => (x.ObjectName, x.Checksum)).ToList();

			if (localButNotOnline.Count != 0)
			{
				Logger.Info($"Found {localButNotOnline.Count} objects that aren't known to the object repository!");

				// would you like to upload?

				foreach (var dat in localButNotOnline)
				{
					await UploadDatToServer(dat);
				}
			}
			else
			{
				Logger.Debug("Found no new objects locally compared to the object repository.");
			}
		}

		public async Task UploadDatToServer(ObjectIndexEntry dat)
		{
			Logger.Info($"Uploading {dat.Filename} to object repository");
			var filename = Path.Combine(Settings.ObjDataDirectory, dat.Filename);
			var lastModifiedTime = File.GetLastWriteTimeUtc(filename); // this is the "Modified" time as shown in Windows
			await Client.UploadDatFileAsync(WebClient, dat.Filename, await File.ReadAllBytesAsync(filename), lastModifiedTime, Logger);
		}
	}
}
