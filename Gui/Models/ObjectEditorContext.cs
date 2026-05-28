using Avalonia.Platform;
using Avalonia.Threading;
using Common;
using Common.Logging;
using Dat.Converters;
using Dat.FileParsing;
using Dat.Types;
using Definitions;
using Definitions.Database;
using Definitions.DTO;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Types;
using DynamicData;
using Gui.Services;
using Microsoft.Extensions.Logging;
using ObjectService.Services;
using SixLabors.ImageSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Gui.Models;

public class ObjectEditorContext : IDisposable, IAsyncDisposable
{
	public EditorSettings Settings { get; private set; }

	public Logger Logger { get; init; }

	public ObjectIndex ObjectIndex { get; set; } = new();

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
	public const string LoggingFileName = "objectEditor.log";

	// stores settings.json, objectEditor.log, etc
	public static string ProgramDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationName);
	public static string SettingsFile => Path.Combine(ProgramDataPath, Environment.GetEnvironmentVariable("ENV_SETTINGS_FILE") ?? EditorSettings.DefaultFileName);
	public static string LoggingFile => Path.Combine(ProgramDataPath, LoggingFileName);

	public ObservableCollection<LogLine> LoggerObservableLogs { get; init; } = [];

	// Dedicated logger for the embedded ObjectService. Routed to its own observable
	// collection so the local-server log popup shows only server-originated lines,
	// while still writing to the shared on-disk log file (with a [LocalServer] tag).
	public Logger LocalServerLogger { get; init; }

	public ObservableCollection<LogLine> LocalServerObservableLogs { get; init; } = [];

	public ObjectServiceClient ObjectServiceClient { get; init; }

	// Second client wired to the configured remote master ObjectService. The GUI talks to
	// both clients via the same HTTP API so local and remote objects can be browsed
	// uniformly; the only difference is which server answered the request.
	public ObjectServiceClient RemoteObjectServiceClient { get; init; }

	public ObjectServiceModel ObjectServiceModel { get; init; }

	// Always non-null. Reflects the lifecycle of the in-process ObjectService - the GUI's
	// only backend. The remote ServerAddress* settings are only used as a fallback if the
	// embedded host fails to start (e.g. paths are misconfigured and unrecoverable).
	public Gui.Services.EmbeddedObjectServiceHost LocalServerHost { get; }

	// Always non-null. Background poller that surfaces the reachability of the configured
	// remote master ObjectService, independently of the embedded local host.
	public Gui.Services.RemoteServerMonitor RemoteServerMonitor { get; }

	// Fires every time the embedded host transitions to Running. Consumers (e.g. post-startup
	// re-indexers) can subscribe to be notified that the local backend is live without
	// having to poll.
	public event EventHandler? LocalServerStarted;

	readonly ConcurrentQueue<string> logQueue = new();
	readonly SemaphoreSlim logFileLock = new(1, 1); // Allow only 1 concurrent write

	public ObjectEditorContext()
	{
		Logger = new Logger();
		LoggerObservableLogs = [];
		Logger.LogAdded += (sender, laea) => LogAsync(laea.Log, LoggerObservableLogs, tag: null).ConfigureAwait(false);

		LocalServerLogger = new Logger();
		LocalServerObservableLogs = [];
		LocalServerLogger.LogAdded += (sender, laea) => LogAsync(laea.Log, LocalServerObservableLogs, tag: "LocalServer").ConfigureAwait(false);

		LoadSettings();

		// settings must be loaded or else the rest of the app cannot start
		ArgumentNullException.ThrowIfNull(Settings);

		Settings.CacheFolder = InitialiseDirectory(Settings.CacheFolder, "cache");
		Settings.DownloadFolder = InitialiseDirectory(Settings.DownloadFolder, "downloads");

		LocalServerHost = new Gui.Services.EmbeddedObjectServiceHost(LocalServerLogger);

		// Make sure the host has a usable objects-root folder and palette file to point
		// at - even if the user hasn't filled them in.
		EnsureLocalServerDefaults();

		// Resolve the loopback URL up front so ObjectServiceClient can be wired to the
		// local host before Kestrel has finished binding. The actual server start is then
		// fire-and-forget on a background task; the client will simply see failed requests
		// until the host transitions to Running.
		var localBaseAddress = PrepareLocalServerUri();

		ObjectServiceClient = new(Settings, Logger, localBaseAddress);
		ObjectServiceModel = new ObjectServiceModel(ObjectServiceClient, Logger);

		// Remote master server: same client class, just pointed at the configured address.
		// Passing null asks the client to resolve the base URL from EditorSettings.
		RemoteObjectServiceClient = new(Settings, Logger, baseAddressOverride: null);

		if (localBaseAddress is not null)
		{
			_ = Task.Run(() => StartLocalServerAsync(localBaseAddress));
		}

		// When the embedded ObjectService finishes coming up, scan the active user folder
		// (if any) so a freshly-created local DB gets populated without manual intervention.
		LocalServerStarted += (_, _) => _ = Task.Run(ReindexConfiguredFoldersAsync);

		RemoteServerMonitor = new Gui.Services.RemoteServerMonitor(Logger);
		RemoteServerMonitor.Configure(GetRemoteServerUri());
	}

	Uri? GetRemoteServerUri()
	{
		var address = Settings.UseHttps ? Settings.ServerAddressHttps : Settings.ServerAddressHttp;
		return Uri.TryCreate(address, UriKind.Absolute, out var uri) ? uri : null;
	}

	// Fills in sensible defaults for the embedded ObjectService when the user hasn't
	// configured paths yet. Lets the feature work out-of-the-box from a clean settings file:
	// the objects root lives under %APPDATA%/.../LocalServer and the palette is extracted
	// from the bundled Avalonia asset on first run.
	void EnsureLocalServerDefaults()
	{
		var changed = false;

		if (string.IsNullOrWhiteSpace(Settings.LocalServerObjectsRoot))
		{
			Settings.LocalServerObjectsRoot = Path.Combine(ProgramDataPath, "LocalServer");
			Logger.LogInformation("Defaulted LocalServerObjectsRoot to \"{Path}\"", Settings.LocalServerObjectsRoot);
			changed = true;
		}

		if (string.IsNullOrWhiteSpace(Settings.LocalServerPaletteMapFile))
		{
			var palettePath = Path.Combine(ProgramDataPath, "palette.png");
			if (!File.Exists(palettePath))
			{
				try
				{
					_ = Directory.CreateDirectory(ProgramDataPath);
					using var src = AssetLoader.Open(new Uri("avares://ObjectEditor/Assets/palette.png"));
					using var dst = File.Create(palettePath);
					src.CopyTo(dst);
					Logger.LogInformation("Extracted bundled palette.png to \"{Path}\"", palettePath);
				}
				catch (Exception ex)
				{
					Logger.LogWarning(ex, "Failed to extract bundled palette.png; the local server will fail to start until LocalServerPaletteMapFile is set manually.");
					return;
				}
			}

			Settings.LocalServerPaletteMapFile = palettePath;
			Logger.LogInformation("Defaulted LocalServerPaletteMapFile to \"{Path}\"", palettePath);
			changed = true;
		}

		if (changed)
		{
			Settings.Save(SettingsFile, Logger);
		}
	}

	// Best-effort reindex of the user's configured obj-data folder(s) into the local DB.
	// Runs on the thread pool, swallows individual-folder errors so one bad path can't
	// stall the whole pass.
	async Task ReindexConfiguredFoldersAsync()
	{
		try
		{
			var folders = new List<string>();
			if (!string.IsNullOrWhiteSpace(Settings.ObjDataDirectory))
			{
				folders.Add(Settings.ObjDataDirectory);
			}

			if (Settings.ObjDataDirectories is { Count: > 0 })
			{
				foreach (var dir in Settings.ObjDataDirectories)
				{
					if (!folders.Contains(dir, StringComparer.OrdinalIgnoreCase))
					{
						folders.Add(dir);
					}
				}
			}

			if (folders.Count == 0)
			{
				Logger.LogInformation("No user obj-data folders configured; skipping post-startup reindex.");
				return;
			}

			// Note: LoadObjDirectoryAsync uses a single shared indexerTask + semaphore, so
			// calls are serialised. RebuildFromFolderAsync wipes the DB between calls, so
			// for now we only fully reindex the *first* (active) folder and log the rest.
			var primary = folders[0];
			if (!Directory.Exists(primary))
			{
				Logger.LogWarning("Configured obj-data folder does not exist: {Directory}", primary);
				return;
			}

			Logger.LogInformation("Reindexing user obj-data folder into local DB: {Directory}", primary);
			var progress = new Progress<float>();
			await LoadObjDirectoryAsync(primary, progress, useExistingIndex: true);

			if (folders.Count > 1)
			{
				Logger.LogInformation("Additional configured folders not auto-reindexed (only the active folder is currently scanned): {Folders}", string.Join(", ", folders.Skip(1)));
			}
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error during post-startup reindex of user folders.");
		}
	}

	// Decides whether the embedded server should be started and, if so, returns the URL it
	// will bind to. The port is reserved synchronously so the caller knows the final URL
	// before Kestrel starts. Returns null only when the required paths cannot be resolved.
	Uri? PrepareLocalServerUri()
	{
		if (string.IsNullOrWhiteSpace(Settings.LocalServerObjectsRoot)
			|| string.IsNullOrWhiteSpace(Settings.LocalServerPaletteMapFile)
			|| string.IsNullOrWhiteSpace(Settings.DatabaseFile))
		{
			Logger.LogWarning("One or more of LocalServerObjectsRoot, LocalServerPaletteMapFile, DatabaseFile is empty. Local server will not start; falling back to remote server.");
			return null;
		}

		var port = Settings.LocalServerPort > 0 ? Settings.LocalServerPort : ReserveLoopbackPort();
		return new Uri($"http://127.0.0.1:{port}/");
	}

	// Briefly opens a TcpListener on port 0 to ask the OS for a free loopback port. There
	// is a small race window between Stop() and Kestrel binding, but for localhost-only
	// startup it's acceptable.
	static int ReserveLoopbackPort()
	{
		var listener = new TcpListener(IPAddress.Loopback, 0);
		listener.Start();
		try
		{
			return ((IPEndPoint)listener.LocalEndpoint).Port;
		}
		finally
		{
			listener.Stop();
		}
	}

	async Task StartLocalServerAsync(Uri targetUri)
	{
		try
		{
			var options = new ObjectService.Hosting.ObjectServiceHostOptions
			{
				RootFolder = Settings.LocalServerObjectsRoot,
				DatabaseFile = Settings.DatabaseFile,
				PaletteMapFile = Settings.LocalServerPaletteMapFile,
				HttpUrl = targetUri.ToString().TrimEnd('/'),
				JwtKey = EmbeddedObjectServiceHost.GenerateEphemeralJwtKey(),
				IsServer = false,
				// Tie the embedded server's lifetime to this GUI process. In-process this is
				// a no-op (same PID); if the host is ever re-architected as a child process
				// the watchdog will reap it when the GUI dies.
				ParentProcessId = Environment.ProcessId,
			};
			await LocalServerHost.StartAsync(options);

			if (LocalServerHost.IsRunning)
			{
				LocalServerStarted?.Invoke(this, EventArgs.Empty);
			}
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Failed to start embedded ObjectService; falling back to remote server for any subsequent calls.");
		}
	}

	// Stops the currently-running embedded host (if any), recomputes the target URL from
	// the current settings, re-targets the ObjectServiceClient, and (if applicable) kicks
	// off a fresh background start. Safe to call from the UI thread.
	public async Task RestartLocalServerAsync()
	{
		try
		{
			await LocalServerHost.StopAsync();
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error while stopping embedded ObjectService during restart.");
		}

		// Pick up any path defaults that may have been cleared by the user.
		EnsureLocalServerDefaults();

		var newBaseAddress = PrepareLocalServerUri();
		ObjectServiceClient.RetargetBaseAddress(newBaseAddress);

		// Remote address may have changed via settings; re-point the monitor and client at the new URI.
		RemoteObjectServiceClient.RetargetBaseAddress(null);
		RemoteServerMonitor.Configure(GetRemoteServerUri());

		if (newBaseAddress is not null)
		{
			_ = Task.Run(() => StartLocalServerAsync(newBaseAddress));
		}
	}

	public Task LogAsync(LogLine log) => LogAsync(log, LoggerObservableLogs, tag: null);

	async Task LogAsync(LogLine log, ObservableCollection<LogLine> sink, string? tag)
	{
		// update UI
		Dispatcher.UIThread.Post(() => sink.Insert(0, log));

		// update log file on disk - prefix server lines so the merged file remains useful.
		var text = tag is null ? log.ToString() : $"[{tag}] {log}";
		logQueue.Enqueue(text);
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

	public bool TryLoadObject(FileSystemItem filesystemItem, out LocoUIObjectModel? uiLocoFile)
	{
		uiLocoFile = null;

		if (string.IsNullOrEmpty(filesystemItem.FileName))
		{
			return false;
		}

		try
		{
			// Dispatch by data source rather than FileLocation: an item with an Id was
			// produced by a server (local or remote) and must be fetched via the matching
			// HTTP client. Items without an Id are raw disk files from the file-open
			// dialog and load straight off disk.
			var result = filesystemItem.Id != null
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
			// Pick the client by FileLocation: Local items came from the embedded server,
			// Online items came from the remote master. The GUI treats them uniformly,
			// but each must round-trip back to its originating server.
			var client = filesystemItem.FileLocation == FileLocation.Online
				? RemoteObjectServiceClient
				: ObjectServiceClient;

			if (client == null)
			{
				Logger.LogError("Object service client is null");
				return false;
			}

			Logger.LogDebug("Didn't find object {DisplayName} with unique id {Id} in cache - downloading it from {BaseAddress}", filesystemItem.DisplayName, filesystemItem.Id, client.WebClient.BaseAddress);

			// Synchronous bridge: TryLoadObject is a sync API used by many callers, so we must
			// block here. Task.Run pushes the await onto the thread pool (no captured UI
			// SynchronizationContext), avoiding the classic UI deadlock. GetAwaiter().GetResult()
			// is preferred over .Result so exceptions are not wrapped in AggregateException.
			cachedLocoObjDto = Task.Run(() => client.GetObjectAsync(filesystemItem.Id.Value)).GetAwaiter().GetResult();

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

	async Task LoadObjDirectoryAsyncCore(string directory, IProgress<float> progress, bool useExistingIndex)
	{
		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory) || progress == null)
		{
			Logger.LogError("Couldn't start loading obj dir: {Directory}", directory);
			return;
		}

		Settings.ObjDataDirectory = directory;
		Settings.Save(SettingsFile, Logger);

		var indexService = new LocalObjectIndexService(
			() => LocoDbContext.GetDbFromFile(Settings.DatabaseFile)
				?? throw new FileNotFoundException($"Database file not found: {Settings.DatabaseFile}"),
			Logger);

		if (useExistingIndex)
		{
			ObjectIndex = await indexService.BuildObjectIndexAsync().ConfigureAwait(false);
			Logger.LogInformation("Loaded index for {Directory} with {Count} objects from {Db}.", directory, ObjectIndex.Objects.Count, Settings.DatabaseFile);

			// Reconcile against on-disk files; rescan deltas.
			var indexed = ObjectIndex.Objects.Select(x => x.FileName).Where(x => !string.IsNullOrEmpty(x)).Cast<string>().ToHashSet();
			var allFiles = SawyerStreamUtils.GetDatFilesInDirectory(directory).ToArray();
			var added = allFiles.Except(indexed).ToList();
			var removed = indexed.Except(allFiles).ToList();
			if (added.Count > 0 || removed.Count > 0)
			{
				Logger.LogWarning("Index and files on disk don't match; rebuilding from folder.");
				Logger.LogWarning("Objects in index but not on disk: {Value}", string.Join(',', removed));
				Logger.LogWarning("Objects on disk but not in index: {Value}", string.Join(',', added));
				await indexService.RebuildFromFolderAsync(directory, progress).ConfigureAwait(false);
				ObjectIndex = await indexService.BuildObjectIndexAsync().ConfigureAwait(false);
			}
		}
		else
		{
			Logger.LogInformation("Rebuilding index for {Directory}", directory);
			await indexService.RebuildFromFolderAsync(directory, progress).ConfigureAwait(false);
			ObjectIndex = await indexService.BuildObjectIndexAsync().ConfigureAwait(false);
			Logger.LogInformation("New index has {Count} objects.", ObjectIndex.Objects.Count);
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
		// Stop the embedded ObjectService (if running) before we tear down logging,
		// so any shutdown logs from Kestrel still have somewhere to go.
		try
		{
			await LocalServerHost.DisposeAsync();
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error while stopping embedded ObjectService.");
		}

		try
		{
			await RemoteServerMonitor.DisposeAsync();
		}
		catch (Exception ex)
		{
			Logger.LogError(ex, "Error while stopping remote server monitor.");
		}

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
