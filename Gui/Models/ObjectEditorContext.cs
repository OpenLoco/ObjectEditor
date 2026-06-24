using Avalonia.Threading;
using Common;
using Common.Logging;
using Dat.Converters;
using Dat.FileParsing;
using Dat.Types;
using Definitions.DTO;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Types;
using DynamicData;
using Index;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gui.Models;

public class ObjectEditorContext : IDisposable, IAsyncDisposable
{
	public EditorSettings Settings { get; private set; }

	public Logger Logger { get; init; }

	public ObjectIndex ObjectIndex { get; private set; } = new();

	public ObjectIndex? ObjectIndexOnline { get; set; }

	public Dictionary<UniqueObjectId, DtoObjectPostResponse> OnlineCache { get; } = [];

	public PaletteMap PaletteMap { get; set; } = null!;

	public G1Dat? G1 { get; set; }

	//public Dictionary<string, byte[]> Music { get; } = [];

	//public Dictionary<string, byte[]> MiscellaneousTracks { get; } = [];

	//public Dictionary<string, byte[]> SoundEffects { get; } = [];

	//public Dictionary<string, byte[]> Tutorials { get; } = [];

	//public Collection<string> MiscFiles { get; } = [];

	public const string ApplicationName = "OpenLoco Object Editor";
	public const string SettingsFileName = "settings.json"; // "settings-dev.json" for dev, "settings.json" for prod
	public const string LoggingFileName = "objectEditor.log";
	public const string ImageTableGroupsFileName = "imageTableGroups.json";

	public string DefaultConfigFolder { get; set; } = "config";
	public string DefaultDownloadFolder { get; set; } = "downloads";
	public string DefaultCacheFolder { get; set; } = "cache";
	public string DefaultObjectIndicesFolder { get; set; } = "objectIndices";

	// stores settings.json, objectEditor.log, etc
	public static string ProgramDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
	public static string SettingsFilePathName => Path.Combine(ProgramDataPath, Environment.GetEnvironmentVariable("ENV_SETTINGS_FILE") ?? SettingsFileName);
	public static string LoggingFilePathName => Path.Combine(ProgramDataPath, LoggingFileName);

	public string ImageTableGroupsPathName => Path.Combine(Settings.ConfigFolder, ImageTableGroupsFileName);

	public ObservableCollection<LogLine> LoggerObservableLogs { get; init; } = [];

	public ObjectServiceClient ObjectServiceClient { get; init; }

	public ObjectServiceModel ObjectServiceModel { get; init; }

	readonly ConcurrentQueue<string> logQueue = new();
	readonly SemaphoreSlim logFileLock = new(1, 1); // Allow only 1 concurrent write

	public ObjectEditorContext()
	{
		Logger = new Logger();
		LoggerObservableLogs = [];
		Logger.LogAdded += (sender, laea) => LogAsync(laea.Log).ConfigureAwait(false);

		LoadSettings();

		// settings must be loaded or else the rest of the app cannot start
		ArgumentNullException.ThrowIfNull(Settings);

		Settings.ObjectIndicesFolder = InitialiseDirectory(Settings.ObjectIndicesFolder, "objectIndices");
		Settings.CacheFolder = InitialiseDirectory(Settings.CacheFolder, "cache");
		Settings.DownloadFolder = InitialiseDirectory(Settings.DownloadFolder, "downloads");
		Settings.ConfigFolder = InitialiseDirectory(Settings.ConfigFolder, "config");

		ObjectServiceClient = new(Settings, Logger);
		ObjectServiceModel = new ObjectServiceModel(ObjectServiceClient, Logger);
	}

	public async Task LogAsync(LogLine log)
	{
		// update UI
		Dispatcher.UIThread.Post(() => LoggerObservableLogs.Insert(0, log));

		// update log file on disk
		logQueue.Enqueue(log.ToString());
		await WriteLogsToFileAsync();
	}

	async Task WriteLogsToFileAsync()
	{
		if (logQueue.IsEmpty)
		{
			return;
		}

		if (await logFileLock.WaitAsync(0)) // Non-blocking wait if available.
		{
			try
			{
				while (logQueue.TryDequeue(out var logMessage))
				{
					try
					{
						await File.AppendAllTextAsync(LoggingFilePathName, logMessage + Environment.NewLine);
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
		Settings = EditorSettings.Load(SettingsFilePathName, Logger);

		if (Settings.Validate(Logger))
		{
			Logger.LogInformation("Settings loaded and validated successfully.");
		}
		else
		{
			Logger.LogError("Unable to validate settings file - please delete it and it will be recreated on next editor start-up.");
		}
	}

	string InitialiseDirectory(string folder, string defaultName)
	{
		if (string.IsNullOrEmpty(folder))
		{
			folder = Path.Combine(ProgramDataPath, defaultName);
		}

		if (!Directory.Exists(folder))
		{
			Logger.LogInformation("\"{DefaultName}\" folder doesn't exist; creating now at \"{Folder}\"", defaultName, folder);
			_ = Directory.CreateDirectory(folder);
		}

		return folder;
	}

	public async Task LoadAsync()
		=> await EnsureDefaultImageTableGroupsConfigFileAsync(Logger, ImageTableGroupsPathName);

	static async Task EnsureDefaultImageTableGroupsConfigFileAsync(Logger logger, string imageTableGroupsPathName)
	{
		logger.LogInformation("Attempting to load image table group config from '{ImageTableGroupsFileName}'", imageTableGroupsPathName);
		var defaultImageTableGroups = await ReadDefaultImageTableGroupsConfigAsync(logger, imageTableGroupsPathName);
		if (defaultImageTableGroups == null)
		{
			logger.LogError("Failed to load default image table group configuration - groups will not be automatically created for existing images. Please ensure the default config file is present and valid at '{ImageTableGroupsFileName}'", imageTableGroupsPathName);
			return;
		}

		var currentImageTableGroups = defaultImageTableGroups;

		if (File.Exists(imageTableGroupsPathName))
		{
			var jsonVersion = ImageTableGrouper.ReadImageTableGroupVersion(logger, imageTableGroupsPathName);
			if (jsonVersion == null || jsonVersion < VersionHelpers.GetCurrentAppVersion())
			{
				currentImageTableGroups = defaultImageTableGroups;
			}
			else
			{
				await File.WriteAllTextAsync(imageTableGroupsPathName, defaultImageTableGroups);
			}
		}
		else
		{
			await File.WriteAllTextAsync(imageTableGroupsPathName, defaultImageTableGroups);
		}

		ImageTableGrouper.LoadGroupConfigurationFile(logger, currentImageTableGroups);
	}

	static async Task<string?> ReadDefaultImageTableGroupsConfigAsync(Logger logger, string imageTableGroupsFileName)
	{
		try
		{
			var assembly = Assembly.GetExecutingAssembly();
			var currentVersion = VersionHelpers.GetCurrentAppVersion();
			using var assemblyStream = assembly.GetManifestResourceStream("Gui.ImageTableGroups.json");
			if (assemblyStream == null)
			{
				logger.LogError("Default image table group configuration resource not found.");
				return null;
			}

			using (var reader = new StreamReader(assemblyStream, leaveOpen: true))
			{
				return await reader.ReadToEndAsync();
			}
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Failed to create default image table group config file.");
			return null;
		}
	}

	public bool TryLoadObject(FileSystemItem filesystemItem, out LocoUIObjectModel? uiLocoFile)
	{
		uiLocoFile = null;

		if (string.IsNullOrEmpty(filesystemItem.FileName))
		{
			return false;
		}

		try
		{
			var result = filesystemItem.FileLocation == FileLocation.Online
				? TryLoadOnlineFile(filesystemItem, out uiLocoFile)
				: TryLoadLocalFile(filesystemItem, out uiLocoFile);

			if (uiLocoFile?.LocoObject == null)
			{
				Logger.LogWarning("Unable to load LocoObject for {FileName}", filesystemItem.FileName);
			}

			if (uiLocoFile?.Metadata == null)
			{
				Logger.LogWarning("Unable to load Metadata for {FileName}", filesystemItem.FileName);
			}

			if (uiLocoFile?.DatInfo == null)
			{
				Logger.LogWarning("Unable to load DatInfo for {FileName}", filesystemItem.FileName);
			}

			return result;
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Unable to load {FileName}", filesystemItem.FileName);
			uiLocoFile = null;
			return false;
		}
	}

	bool TryLoadOnlineFile(FileSystemItem filesystemItem, out LocoUIObjectModel? locoDatFile)
	{
		locoDatFile = null;

		DatHeaderInfo? fileInfo = null;
		LocoObject? locoObject = null;
		ObjectMetadata? metadata = null;
		//List<Image<Rgba32>> images = [];

		if (filesystemItem.Id == null)
		{
			return false;
		}

		if (!OnlineCache.TryGetValue(filesystemItem.Id.Value, out var cachedLocoObjDto)) // issue - if an object doesn't download its full file, it's 'header' will remain in cache but unable to attempt redownload
		{
			Logger.LogDebug("Didn't find object {DisplayName} with unique id {Id} in cache - downloading it from {BaseAddress}", filesystemItem.DisplayName, filesystemItem.Id, ObjectServiceClient.WebClient.BaseAddress);

			if (ObjectServiceClient == null)
			{
				Logger.LogError("Object service client is null");
				return false;
			}

			// Synchronous bridge: TryLoadObject is a sync API used by many callers, so we must
			// block here. Task.Run pushes the await onto the thread pool (no captured UI
			// SynchronizationContext), avoiding the classic UI deadlock. GetAwaiter().GetResult()
			// is preferred over .Result so exceptions are not wrapped in AggregateException.
			cachedLocoObjDto = Task.Run(() => ObjectServiceClient.GetObjectAsync(filesystemItem.Id.Value)).GetAwaiter().GetResult();

			if (cachedLocoObjDto == null)
			{
				Logger.LogError("Unable to download object {DisplayName} with unique id {Id} from online - received no data", filesystemItem.DisplayName, filesystemItem.Id);
				return false;
			}

			Logger.LogDebug(cachedLocoObjDto.ToString());
			Logger.LogInformation("Object {DisplayName} with unique id {Id} has {Count} attached DAT objects: [{Value}]", filesystemItem.DisplayName, filesystemItem.Id, cachedLocoObjDto.DatObjects.Count, string.Join(',', cachedLocoObjDto.DatObjects));

			foreach (var datObject in cachedLocoObjDto.DatObjects)
			{
				if (cachedLocoObjDto.ObjectSource is ObjectSource.LocomotionSteam or ObjectSource.LocomotionGoG)
				{
					Logger.LogWarning("Unable to download object {DisplayName} with unique id {Id} from online - requested object is a vanilla object and it is illegal to distribute copyright material. Any available metadata will still be shown", filesystemItem.DisplayName, filesystemItem.Id);
					continue;
				}

				if (string.IsNullOrEmpty(datObject.DatBytesAsBase64))
				{
					Logger.LogWarning("Unable to download object {DisplayName} with unique id {Id} from online - received no DAT object data. Any available metadata will still be shown", filesystemItem.DisplayName, filesystemItem.Id);
					continue;
				}

				var datFile = Convert.FromBase64String(datObject.DatBytesAsBase64);
				if (datFile == null || datFile.Length == 0)
				{
					Logger.LogWarning("Unable to download object {DisplayName} with unique id {Id} from online - received DAT object data, but it was unable to be decoded. Any available metadata will still be shown", filesystemItem.DisplayName, filesystemItem.Id);
					continue;
				}

				var filename = Path.GetInvalidFileNameChars().Aggregate(datObject.DatName, (current, c) => current.Replace(c, '_'));
				filename = $"{filename}-{datObject.ObjectId}.dat";
				var pathname = Path.Combine(Settings.CacheFolder, filename);
				if (!File.Exists(pathname))
				{
					File.WriteAllBytes(pathname, datFile);
					Logger.LogInformation("Saved the downloaded object {DisplayName} with unique id {Id} as {Pathname}", filesystemItem.DisplayName, filesystemItem.Id, pathname);

					//var obj = SawyerStreamReader.LoadFullObjectFromStream(datFile, Logger, $"{filesystemItem.Filename}-{filesystemItem.DisplayName}", true);
					//fileInfo = obj.DatFileInfo;
					//locoObject = obj.LocoObject;
					//if (obj.LocoObject == null)
					//{
					//	Logger.Warning("Unable to load {DisplayName} from the received DAT object data", filesystemItem.DisplayName);
					//}
				}
			}

			Logger.LogInformation("Downloaded object \"{DisplayName}\" with unique id {Id} and added it to the local cache", filesystemItem.DisplayName, filesystemItem.Id);
			Logger.LogDebug("{DisplayName} has authors=[{Value}], tags=[{Value2}], objectpacks=[{Value3}], licence={Licence} datobjects=[{Value4}]", filesystemItem.DisplayName, string.Join(", ", cachedLocoObjDto?.Authors?.Select(x => x.Name) ?? []), string.Join(", ", cachedLocoObjDto?.Tags?.Select(x => x.Name) ?? []), string.Join(", ", cachedLocoObjDto?.ObjectPacks?.Select(x => x.Name) ?? []), cachedLocoObjDto?.Licence, string.Join(",", cachedLocoObjDto?.DatObjects?.Select(x => x.DatName) ?? []));

			OnlineCache.Add(filesystemItem.Id.Value, cachedLocoObjDto!);
		}
		else
		{
			Logger.LogDebug("Found object {DisplayName} with unique id {Id} in cache - reusing it", filesystemItem.DisplayName, filesystemItem.Id);
		}

		if (cachedLocoObjDto != null)
		{
			var firstLinkedDatFile = cachedLocoObjDto!.DatObjects.First();
			if (firstLinkedDatFile.DatBytesAsBase64?.Length > 0)
			{
				var obj = SawyerStreamReader.LoadFullObject(
					Convert.FromBase64String(firstLinkedDatFile.DatBytesAsBase64),
					Logger,
					filename: $"{filesystemItem.FileName}-{filesystemItem.DisplayName}",
					loadExtra: true);

				fileInfo = obj.DatFileInfo;
				locoObject = obj.LocoObject;
				if (obj.LocoObject == null)
				{
					Logger.LogWarning("Unable to load {DisplayName} from the received DAT object data", filesystemItem.DisplayName);
				}
			}
			else
			{
				Logger.LogWarning("Cached object {DisplayName} had no data in DatBytes", filesystemItem.DisplayName);
				var fakeS5Header = new S5Header(0, firstLinkedDatFile.DatName, firstLinkedDatFile.DatChecksum)
				{
					ObjectType = cachedLocoObjDto.ObjectType.Convert(),
					ObjectSource = cachedLocoObjDto.ObjectSource.Convert()
				};
				fileInfo = new DatHeaderInfo(fakeS5Header, ObjectHeader.NullHeader);
			}

			metadata = new ObjectMetadata(cachedLocoObjDto.Name)
			{
				UniqueObjectId = cachedLocoObjDto.Id,
				Description = cachedLocoObjDto.Description,
				Authors = [.. cachedLocoObjDto.Authors],
				CreatedDate = cachedLocoObjDto.CreatedDate?.ToDateTimeOffset(),
				ModifiedDate = cachedLocoObjDto.ModifiedDate?.ToDateTimeOffset(),
				UploadedDate = cachedLocoObjDto.UploadedDate.ToDateTimeOffset(),
				Tags = [.. cachedLocoObjDto.Tags],
				ObjectPacks = [.. cachedLocoObjDto.ObjectPacks],
				DatObjects = [.. cachedLocoObjDto.DatObjects],
				Licence = cachedLocoObjDto.Licence,
				Availability = cachedLocoObjDto.Availability,
				//SubObject = cachedLocoObjDto.SubObject,
			};

			//if (locoObject != null)
			//{
			//	foreach (var i in locoObject.GraphicsElements)
			//	{
			//		if (PaletteMap.TryConvertG1ToRgba32Bitmap(i, ColourRemapSwatch.PrimaryRemap, ColourRemapSwatch.SecondaryRemap, out var image))
			//		{
			//			images.Add(image!);
			//		}
			//	}
			//}
		}

		locoDatFile = new LocoUIObjectModel() { DatInfo = fileInfo, LocoObject = locoObject, Metadata = metadata };
		return true;
	}

	bool TryLoadLocalFile(FileSystemItem filesystemItem, out LocoUIObjectModel? locoDatFile)
	{
		locoDatFile = null;

		DatHeaderInfo? fileInfo = null;
		LocoObject? locoObject = null;
		ObjectMetadata? metadata = null;

		var filename = File.Exists(filesystemItem.FileName)
			? filesystemItem.FileName
			: Path.Combine(Settings.ObjDataDirectory, filesystemItem.FileName ?? string.Empty);

		var obj = SawyerStreamReader.LoadFullObject(filename, logger: Logger);
		fileInfo = obj.DatFileInfo;
		locoObject = obj.LocoObject;
		metadata = new ObjectMetadata("<none>")
		{
			CreatedDate = filesystemItem.CreatedDate?.ToDateTimeOffset(),
			ModifiedDate = filesystemItem.ModifiedDate?.ToDateTimeOffset(),
			Availability = Definitions.ObjectAvailability.Available,
			//DatObjects = [new(0)],
		}; // todo: look up the rest of the data from internet

		locoDatFile = new LocoUIObjectModel() { DatInfo = fileInfo, LocoObject = locoObject, Metadata = metadata };
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

	public string IndexFileName
	{
		get
		{
			var filename = Convert.ToBase64String(Encoding.UTF8.GetBytes(Settings.ObjDataDirectory));
			return Path.Combine(Settings.ObjectIndicesFolder, $"{filename}.json");
		}
	}

	async Task LoadObjDirectoryAsyncCore(string directory, IProgress<float> progress, bool useExistingIndex)
	{
		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory) || progress == null)
		{
			Logger.LogError("Couldn't start loading obj dir: {Directory}", directory);
			return;
		}

		Settings.ObjDataDirectory = directory;
		Settings.Save(SettingsFilePathName, Logger);

		if (useExistingIndex && File.Exists(IndexFileName))
		{
			var exception = false;

			try
			{
				var index = await ObjectIndex.LoadIndexAsync(IndexFileName).ConfigureAwait(false);
				ArgumentNullException.ThrowIfNull(index, nameof(index));
				ObjectIndex = index;
				Logger.LogInformation("Loaded index for {Directory} with {Count} objects.", directory, ObjectIndex.Objects.Count);
			}
			catch (Exception ex)
			{
				Logger.LogError(ex, "Failed to load index from \"{IndexFileName}\"", IndexFileName);
				exception = true;
			}

			if (exception || ObjectIndex?.Objects == null || ObjectIndex.Objects.Any(x => string.IsNullOrEmpty(x.FileName) || (x is ObjectIndexEntry xx && string.IsNullOrEmpty(xx.DisplayName))))
			{
				Logger.LogWarning("Index file format has changed or otherwise appears to be malformed - recreating now.");
				await RecreateIndex(directory, progress).ConfigureAwait(false);
				return;
			}

			var objectIndexFilenames = ObjectIndex.Objects.Select(x => x.FileName);
			var allFiles = SawyerStreamUtils.GetDatFilesInDirectory(directory).ToArray();

			var a = objectIndexFilenames.Except(allFiles);
			var b = allFiles.Except(objectIndexFilenames);
			if (a.Any() || b.Any())
			{
				Logger.LogWarning("Index file and files on disk don't match; re-indexing those files and updating the index now.");
				Logger.LogWarning("Objects in index but not on disk: {Value}", string.Join(',', a));
				Logger.LogWarning("Objects on disk but not in index: {Value}", string.Join(',', b));
				await UpdateIndex(directory, progress, a.Concat(b).Where(x => x != null)!).ConfigureAwait(false);
			}
		}
		else
		{
			await RecreateIndex(directory, progress).ConfigureAwait(false);
		}

		async Task UpdateIndex(string directory, IProgress<float> progress, IEnumerable<string> filesToAdd)
		{
			Logger.LogInformation("Updating index file for {Directory}", directory);
			_ = ObjectIndex.UpdateIndex(directory, Logger, filesToAdd, progress);

			if (string.IsNullOrEmpty(IndexFileName))
			{
				Logger.LogError("Index filename was null or empty.");
				return;
			}

			await ObjectIndex.SaveIndexAsync(IndexFileName).ConfigureAwait(false);
			Logger.LogInformation("Index was saved to {IndexFileName}", IndexFileName);
		}

		async Task RecreateIndex(string directory, IProgress<float> progress)
		{
			Logger.LogInformation("Recreating index file for {Directory}", directory);
			ObjectIndex = await ObjectIndex.CreateIndexAsync(directory, Logger, progress).ConfigureAwait(false);

			if (ObjectIndex == null)
			{
				Logger.LogError("Index was unable to be created.");
				return;
			}

			if (string.IsNullOrEmpty(IndexFileName))
			{
				Logger.LogError("Index filename was null or empty.");
				return;
			}

			await ObjectIndex.SaveIndexAsync(IndexFileName).ConfigureAwait(false);
			Logger.LogInformation("New index was saved to {IndexFileName}", IndexFileName);
		}
	}

	public async Task CheckForDatFilesNotOnServer()
	{
		if (ObjectIndex == null || ObjectIndexOnline == null || ObjectIndexOnline.Objects.Count == 0)
		{
			return;
		}

		Logger.LogDebug("Comparing local objects to object repository");

		var localButNotOnline = ObjectIndex.Objects.ExceptBy(ObjectIndexOnline.Objects.Select(
			x => (x.DisplayName, x.DatChecksum)),
			x => (x.DisplayName, x.DatChecksum)).ToList();

		if (localButNotOnline.Count != 0)
		{
			Logger.LogInformation("Found {Count} objects that aren't known to the object repository!", localButNotOnline.Count);

			// would you like to upload?
			var isEnabledString = Settings.AutoObjectDiscoveryAndUpload ? "enabled" : "disabled";
			Logger.LogInformation("Automatic object discovery and upload to master service is {IsEnabledString}", isEnabledString);
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
			Logger.LogDebug("Found no new objects locally compared to the object repository.");
		}
	}

	public async Task UploadDatToServer(ObjectIndexEntry dat)
	{
		Logger.LogInformation("Uploading {FileName} to object repository", dat.FileName);
		var filename = Path.Combine(Settings.ObjDataDirectory, dat.FileName ?? string.Empty);
		var creationDate = DateOnly.FromDateTime(File.GetCreationTimeUtc(filename));
		var modifiedDate = DateOnly.FromDateTime(File.GetLastWriteTimeUtc(filename));

		if (ObjectServiceClient == null)
		{
			Logger.LogError("Object service client is null");
			return;
		}

		// todo: do something with createdObject
		_ = await ObjectServiceClient.UploadDatFileAsync(dat.FileName ?? string.Empty, await File.ReadAllBytesAsync(filename), creationDate, modifiedDate);

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
		GC.SuppressFinalize(this);
	}

	public async ValueTask DisposeAsync()
	{
		await CloseAsync().ConfigureAwait(false);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}

		// Synchronous Dispose path: run the async cleanup on the thread pool to avoid any
		// captured UI SynchronizationContext deadlock. Prefer DisposeAsync() where possible.
		try
		{
			Task.Run(CloseAsync).GetAwaiter().GetResult();
		}
		catch
		{
			// Best-effort cleanup; never throw from Dispose.
		}
	}
}
