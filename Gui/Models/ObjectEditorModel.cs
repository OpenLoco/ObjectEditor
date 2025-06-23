using Avalonia.Threading;
using DynamicData;
using OpenLoco.Common.Logging;
using OpenLoco.Dat;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using OpenLoco.Definitions.Database;
using OpenLoco.Definitions.DTO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OpenLoco.Gui.Models
{
	public class ObjectEditorModel : IDisposable
	{
		public EditorSettings Settings { get; private set; }

		public ILogger Logger { get; init; }

		public ObjectIndex ObjectIndex { get; private set; }

		public ObjectIndex? ObjectIndexOnline { get; set; }

		public Dictionary<int, DtoObjectDescriptor> OnlineCache { get; } = [];

		public PaletteMap PaletteMap { get; set; }

		public G1Dat? G1 { get; set; }

		public Dictionary<string, byte[]> Music { get; } = [];

		public Dictionary<string, byte[]> MiscellaneousTracks { get; } = [];

		public Dictionary<string, byte[]> SoundEffects { get; } = [];

		public Dictionary<string, byte[]> Tutorials { get; } = [];

		public Collection<string> MiscFiles { get; } = [];

		public const string ApplicationName = "OpenLoco Object Editor";
		public const string LoggingFileName = "objectEditor.log";

		// stores settings.json, objectEditor.log, etc
		public static string ProgramDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
		public static string SettingsFile => Path.Combine(ProgramDataPath, Environment.GetEnvironmentVariable("ENV_SETTINGS_FILE") ?? EditorSettings.DefaultFileName);
		public static string LoggingFile => Path.Combine(ProgramDataPath, LoggingFileName);

		public ObservableCollection<LogLine> LoggerObservableLogs = [];

		public ObjectServiceClient ObjectServiceClient { get; init; }

		readonly ConcurrentQueue<string> logQueue = new();
		readonly SemaphoreSlim logFileLock = new(1, 1); // Allow only 1 concurrent write

		public ObjectEditorModel()
		{
			Logger = new Logger();
			LoggerObservableLogs = [];
			Logger.LogAdded += (sender, laea) => Dispatcher.UIThread.Post(() => LoggerObservableLogs.Insert(0, laea.Log));
			Logger.LogAdded += (sender, laea) => LogAsync(laea.Log.ToString()).ConfigureAwait(false);

			LoadSettings();

			// settings must be loaded or else the rest of the app cannot start
			ArgumentNullException.ThrowIfNull(Settings);

			InitialiseDownloadDirectory();
			ObjectServiceClient = new(Settings, Logger);
		}

		public async Task LogAsync(string message)
		{
			logQueue.Enqueue(message);
			await WriteLogsToFileAsync(); // Start the async writing process
		}

		async Task WriteLogsToFileAsync()
		{
			if (logQueue.IsEmpty)
			{
				return; // Nothing to write
			}

			if (await logFileLock.WaitAsync(0)) // Non-blocking wait if available.
			{
				try
				{
					while (logQueue.TryDequeue(out var logMessage))
					{
						try
						{
							await File.AppendAllTextAsync(LoggingFile, logMessage + Environment.NewLine);
						}
						catch (Exception ex)
						{
							Console.WriteLine($"Error writing to log file: {ex.Message}");
							// Consider logging to a separate error file or using a more robust logging framework
						}
					}
				}
				finally
				{
					_ = logFileLock.Release(); // Release the semaphore
				}
			}
		}

		void LoadSettings()
		{
			Settings = EditorSettings.Load(SettingsFile, Logger);

			if (Settings.Validate(Logger))
			{
				Logger.Info("Settings loaded and validated successfully.");
			}
			else
			{
				Logger.Error("Unable to validate settings file - please delete it and it will be recreated on next editor start-up.");
			}
		}

		void InitialiseDownloadDirectory()
		{
			if (string.IsNullOrEmpty(Settings.DownloadFolder))
			{
				Settings.DownloadFolder = Path.Combine(ProgramDataPath, "downloads");
			}

			if (!Directory.Exists(Settings.DownloadFolder))
			{
				Logger.Info($"Download folder doesn't exist; creating now at \"{Settings.DownloadFolder}\"");
				_ = Directory.CreateDirectory(Settings.DownloadFolder);
			}
		}

		public bool TryLoadObject(FileSystemItemBase filesystemItem, out UiDatLocoFile? uiLocoFile)
		{
			uiLocoFile = null;

			if (string.IsNullOrEmpty(filesystemItem.Filename))
			{
				return false;
			}

			try
			{
				var result = filesystemItem.FileLocation == FileLocation.Online
					? TryLoadOnlineFile(filesystemItem, out uiLocoFile)
					: TryLoadLocalFile(filesystemItem, out uiLocoFile);

				if (uiLocoFile?.DatFileInfo == null)
				{
					Logger.Error($"Unable to load {filesystemItem.Filename}");
					uiLocoFile = null;
					return false;
				}

				return result;
			}
			catch (Exception ex)
			{
				Logger.Error($"Unable to load {filesystemItem.Filename}", ex);
				uiLocoFile = null;
				return false;
			}
		}

		bool TryLoadOnlineFile(FileSystemItemBase filesystemItem, out UiDatLocoFile? locoDatFile)
		{
			locoDatFile = null;

			DatFileInfo? fileInfo = null;
			ILocoObject? locoObject = null;
			MetadataModel? metadata = null;
			List<Image<Rgba32>> images = [];

			var uniqueObjectId = int.Parse(Path.GetFileNameWithoutExtension(filesystemItem.Filename));

			if (!OnlineCache.TryGetValue(uniqueObjectId, out var cachedLocoObjDto)) // issue - if an object doesn't download its full file, it's 'header' will remain in cache but unable to attempt redownload
			{
				if (ObjectServiceClient == null)
				{
					Logger.Error("Object service client is null");
					return false;
				}

				Logger.Debug($"Didn't find object {filesystemItem.DisplayName} with unique id {uniqueObjectId} in cache - downloading it from {ObjectServiceClient.WebClient.BaseAddress}");
				cachedLocoObjDto = Task.Run(async () => await ObjectServiceClient.GetObjectAsync(uniqueObjectId)).Result;

				if (cachedLocoObjDto == null)
				{
					Logger.Error($"Unable to download object {filesystemItem.DisplayName} with unique id {uniqueObjectId} from online - received no data");
					return false;
				}
				else if (string.IsNullOrEmpty(cachedLocoObjDto.DatObjects.FirstOrDefault()?.DatBytes))
				{
					if (cachedLocoObjDto.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
					{
						Logger.Warning("This is a vanilla object. The DAT file cannot be downloaded due to copyright. Any available metadata will still be shown.");
					}

					Logger.Warning($"Unable to download object {filesystemItem.DisplayName} with unique id {uniqueObjectId} from online - received no DAT object data. Any available metadata will still be shown.");
				}
				else if (cachedLocoObjDto.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
				{
					Logger.Warning($"Unable to download object {filesystemItem.DisplayName} with unique id {uniqueObjectId} from online - requested object is a vanilla object and it is illegal to distribute copyright material. Any available metadata will still be shown.");
				}

				Logger.Debug(cachedLocoObjDto.ToString());

				var datFile = Convert.FromBase64String(cachedLocoObjDto.DatObjects.First().DatBytes);
				if (datFile == null || datFile.Length == 0)
				{
					Logger.Warning($"Unable to download object {filesystemItem.DisplayName} with unique id {uniqueObjectId} from online - received no DAT object data. Any available metadata will still be shown.");
				}
				else
				{
					var filename = Path.Combine(Settings.DownloadFolder, $"{cachedLocoObjDto.InternalName}.dat");
					if (!File.Exists(filename))
					{
						File.WriteAllBytes(filename, datFile);
						Logger.Info($"Saved the downloaded object {filesystemItem.DisplayName} with unique id {uniqueObjectId} as {filename}");

						var obj = SawyerStreamReader.LoadFullObjectFromStream(datFile, Logger, $"{filesystemItem.Filename}-{filesystemItem.DisplayName}", true);
						fileInfo = obj.DatFileInfo;
						locoObject = obj.LocoObject;
						if (obj.LocoObject == null)
						{
							Logger.Warning($"Unable to load {filesystemItem.DisplayName} from the received DAT object data");
						}
					}
				}

				Logger.Info($"Downloaded object \"{filesystemItem.DisplayName}\" with unique id {uniqueObjectId} and added it to the local cache");
				Logger.Debug($"{filesystemItem.DisplayName} has authors=[{string.Join(", ", cachedLocoObjDto?.Authors?.Select(x => x.Name) ?? [])}], tags=[{string.Join(", ", cachedLocoObjDto?.Tags?.Select(x => x.Name) ?? [])}], objectpacks=[{string.Join(", ", cachedLocoObjDto?.ObjectPacks?.Select(x => x.Name) ?? [])}], licence={cachedLocoObjDto?.Licence}");

				OnlineCache.Add(uniqueObjectId, cachedLocoObjDto!);
			}
			else
			{
				Logger.Debug($"Found object {filesystemItem.DisplayName} with unique id {uniqueObjectId} in cache - reusing it");
			}

			if (cachedLocoObjDto != null)
			{
				var firstLinkedDatFile = cachedLocoObjDto!.DatObjects.First();
				if (firstLinkedDatFile.DatBytes?.Length > 0)
				{
					var obj = SawyerStreamReader.LoadFullObjectFromStream(Convert.FromBase64String(firstLinkedDatFile.DatBytes), Logger, $"{filesystemItem.Filename}-{filesystemItem.DisplayName}", true);
					fileInfo = obj.DatFileInfo;
					locoObject = obj.LocoObject;
					if (obj.LocoObject == null)
					{
						Logger.Warning($"Unable to load {filesystemItem.DisplayName} from the received DAT object data");
					}
				}
				else
				{
					Logger.Error($"Caches object {filesystemItem.DisplayName} had no data in DatBytes");
				}

				metadata = new MetadataModel(cachedLocoObjDto.InternalName)
				{
					Description = cachedLocoObjDto.Description,
					Authors = [.. cachedLocoObjDto.Authors.Select(x => x.ToTable())],
					CreatedDate = cachedLocoObjDto.CreatedDate,
					ModifiedDate = cachedLocoObjDto.ModifiedDate,
					UploadedDate = cachedLocoObjDto.UploadedDate,
					Tags = [.. cachedLocoObjDto.Tags.Select(x => x.ToTable())],
					ObjectPacks = [.. cachedLocoObjDto.ObjectPacks.Select(x => x.ToTable())],
					DatObjects = [.. cachedLocoObjDto.DatObjects.Select(x => x.ToTable())],
					Licence = cachedLocoObjDto.Licence?.ToTable(),
				};

				if (locoObject != null)
				{
					foreach (var i in locoObject.G1Elements)
					{
						if (PaletteMap.TryConvertG1ToRgba32Bitmap(i, ColourRemapSwatch.PrimaryRemap, ColourRemapSwatch.SecondaryRemap, out var image))
						{
							images.Add(image!);
						}
					}
				}
			}

			locoDatFile = new UiDatLocoFile() { DatFileInfo = fileInfo, LocoObject = locoObject, Metadata = metadata, Images = images };
			return true;
		}

		bool TryLoadLocalFile(FileSystemItemBase filesystemItem, out UiDatLocoFile? locoDatFile)
		{
			locoDatFile = null;

			DatFileInfo? fileInfo = null;
			ILocoObject? locoObject = null;
			MetadataModel? metadata = null;
			List<Image<Rgba32>> images = [];

			var filename = File.Exists(filesystemItem.Filename)
				? filesystemItem.Filename
				: Path.Combine(Settings.ObjDataDirectory, filesystemItem.Filename);

			var obj = SawyerStreamReader.LoadFullObjectFromFile(filename, logger: Logger);
			if (obj != null)
			{
				fileInfo = obj.Value.DatFileInfo;
				locoObject = obj.Value.LocoObject;
				metadata = new MetadataModel("<unknown>")
				{
					CreatedDate = filesystemItem.CreatedDate,
					ModifiedDate = filesystemItem.ModifiedDate
				}; // todo: look up the rest of the data from internet

				if (locoObject != null)
				{
					foreach (var i in locoObject.G1Elements)
					{
						if (PaletteMap.TryConvertG1ToRgba32Bitmap(i, ColourRemapSwatch.PrimaryRemap, ColourRemapSwatch.SecondaryRemap, out var image))
						{
							images.Add(image!);
						}
					}
				}
			}

			locoDatFile = new UiDatLocoFile() { DatFileInfo = fileInfo, LocoObject = locoObject, Metadata = metadata, Images = images };
			return true;
		}

		static Task? indexerTask;
		static readonly SemaphoreSlim taskLock = new(1, 1);

		public async Task LoadObjDirectoryAsync(string directory, IProgress<float> progress, bool useExistingIndex)
		{
			// Check if a task is already running WITHOUT waiting on the semaphore
			if (indexerTask?.IsCompleted == false)
			{
				// A task is already running, so just return the existing task
				return; // Or return _indexerTask if you need to await it elsewhere
			}

			// Only acquire the semaphore if no task is running
			await taskLock.WaitAsync();
			try
			{
				//Double check inside the lock
				if (indexerTask?.IsCompleted == false)
				{
					return;
				}

				indexerTask = Task.Run(async () => await LoadObjDirectoryAsyncCore(directory, progress, useExistingIndex));
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
			Settings.Save(SettingsFile, Logger);

			if (useExistingIndex && File.Exists(Settings.IndexFileName))
			{
				var exception = false;

				try
				{
					var index = await ObjectIndex.LoadIndexAsync(Settings.IndexFileName).ConfigureAwait(false);
					ArgumentNullException.ThrowIfNull(index, nameof(index));
					ObjectIndex = index;
					Logger.Info($"Loaded index for {directory} with {ObjectIndex.Objects.Count} objects.");
				}
				catch (Exception ex)
				{
					Logger.Error(ex);
					exception = true;
				}

				if (exception || ObjectIndex?.Objects == null || ObjectIndex.Objects.Any(x => string.IsNullOrEmpty(x.Filename) || (x is ObjectIndexEntry xx && string.IsNullOrEmpty(xx.DisplayName))))
				{
					Logger.Warning("Index file format has changed or otherwise appears to be malformed - recreating now.");
					await RecreateIndex(directory, progress).ConfigureAwait(false);
					return;
				}

				var objectIndexFilenames = ObjectIndex.Objects.Select(x => x.Filename);
				var allFiles = SawyerStreamUtils.GetDatFilesInDirectory(directory).ToArray();

				var a = objectIndexFilenames.Except(allFiles);
				var b = allFiles.Except(objectIndexFilenames);
				if (a.Any() || b.Any())
				{
					Logger.Warning("Index file and files on disk don't match; re-indexing those files and updating the index now.");
					Logger.Warning($"Objects in index but not on disk: {string.Join(',', a)}");
					Logger.Warning($"Objects on disk but not in index: {string.Join(',', b)}");
					await UpdateIndex(directory, progress, a.Concat(b)).ConfigureAwait(false);
				}
			}
			else
			{
				await RecreateIndex(directory, progress).ConfigureAwait(false);
			}

			async Task UpdateIndex(string directory, IProgress<float> progress, IEnumerable<string> filesToAdd)
			{
				Logger.Info($"Updating index file for {directory}");
				_ = ObjectIndex.UpdateIndex(directory, Logger, filesToAdd, progress);

				if (string.IsNullOrEmpty(Settings.IndexFileName))
				{
					Logger.Error("Index filename was null or empty.");
					return;
				}

				await ObjectIndex.SaveIndexAsync(Settings.IndexFileName).ConfigureAwait(false);
				Logger.Info($"Index was saved to {Settings.IndexFileName}");
			}

			async Task RecreateIndex(string directory, IProgress<float> progress)
			{
				Logger.Info($"Recreating index file for {directory}");
				ObjectIndex = await ObjectIndex.CreateIndexAsync(directory, Logger, progress).ConfigureAwait(false);

				if (ObjectIndex == null)
				{
					Logger.Error("Index was unable to be created.");
					return;
				}

				if (string.IsNullOrEmpty(Settings.IndexFileName))
				{
					Logger.Error("Index filename was null or empty.");
					return;
				}

				await ObjectIndex.SaveIndexAsync(Settings.IndexFileName).ConfigureAwait(false);
				Logger.Info($"New index was saved to {Settings.IndexFileName}");
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
				x => (x.DisplayName, x.DatChecksum)),
				x => (x.DisplayName, x.DatChecksum)).ToList();

			if (localButNotOnline.Count != 0)
			{
				Logger.Info($"Found {localButNotOnline.Count} objects that aren't known to the object repository!");

				// would you like to upload?
				var isEnabledString = Settings.AutoObjectDiscoveryAndUpload ? "enabled" : "disabled";
				Logger.Info($"Automatic object discovery and upload to master service is {isEnabledString}");
				if (Settings.AutoObjectDiscoveryAndUpload)
				{
					foreach (var dat in localButNotOnline)
					{
						await UploadDatToServer(dat);
					}
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
			var creationDate = File.GetCreationTimeUtc(filename);
			var modifiedDate = File.GetLastWriteTimeUtc(filename);

			if (ObjectServiceClient == null)
			{
				Logger.Error("Object service client is null");
				return;
			}

			await ObjectServiceClient.UploadDatFileAsync(dat.Filename, await File.ReadAllBytesAsync(filename), creationDate, modifiedDate);
			await Task.Delay(100); // wait 100ms, ie don't DoS the server
		}

		public async Task CloseAsync()
		{
			// Wait for any pending writes to complete.
			await logFileLock.WaitAsync(); // Acquire the semaphore
			_ = logFileLock.Release(); // Release it immediately after. This is just to wait.

			// Process any remaining logs in the queue.
			await WriteLogsToFileAsync(); // One last flush.

			logFileLock.Dispose(); // Dispose of the semaphore
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this); // Important for proper disposal
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// Wait for logging to complete synchronously.
				Task.Run(CloseAsync).Wait(); // <--- Key change
				logFileLock?.Dispose(); // Dispose of the semaphore
			}
		}
	}
}
