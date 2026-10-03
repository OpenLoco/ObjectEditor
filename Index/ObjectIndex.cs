using Common.Json;
using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels.Objects.Vehicle;
using Definitions.ObjectModels.Types;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO.Hashing;
using System.Text.Json.Serialization;

namespace Index;

public class ObjectIndex
{
	public ObservableCollection<ObjectIndexEntry> Objects { get; init; } = [];

	[JsonIgnore]
	public const string DefaultIndexFileName = "objectIndex.json";

	[JsonIgnore]
	public const string DefaultIndexDbFileName = "objectIndex.db";

	private readonly ConcurrentDictionary<ulong, ObjectIndexEntry> _byXxHash3 = new();
	private readonly ConcurrentDictionary<(string DisplayName, uint Checksum), ObjectIndexEntry> _byNameChecksum = new();

	public ObjectIndex()
	{ }

	public ObjectIndex(ObservableCollection<ObjectIndexEntry> objects)
	{
		Objects = objects;
		foreach (var entry in Objects)
		{
			AddToLookups(entry);
		}
	}

	public ObjectIndex(IEnumerable<ObjectIndexEntry> objects)
	{
		Objects = [.. objects];
		foreach (var entry in Objects)
		{
			AddToLookups(entry);
		}
	}

	private void AddToLookups(ObjectIndexEntry entry)
	{
		if (entry.xxHash3.HasValue)
		{
			_byXxHash3[entry.xxHash3.Value] = entry;
		}

		if (entry.DatChecksum.HasValue)
		{
			_byNameChecksum[(entry.DisplayName, entry.DatChecksum.Value)] = entry;
		}
	}

	/// <summary>
	/// Rebuilds the internal lookup dictionaries from the <see cref="Objects"/> collection.
	/// Must be called after JSON deserialization since the dictionaries are not serialized.
	/// </summary>
	public void RebuildLookups()
	{
		_byXxHash3.Clear();
		_byNameChecksum.Clear();

		foreach (var entry in Objects)
		{
			AddToLookups(entry);
		}
	}

	private void RemoveFromLookups(ObjectIndexEntry entry)
	{
		if (entry.xxHash3.HasValue)
		{
			var key = entry.xxHash3.Value;
			if (_byXxHash3.TryGetValue(key, out var existing) && ReferenceEquals(existing, entry))
			{
				_ = _byXxHash3.TryRemove(key, out _);
			}
		}

		if (entry.DatChecksum.HasValue)
		{
			var key = (entry.DisplayName, entry.DatChecksum.Value);
			if (_byNameChecksum.TryGetValue(key, out var existing) && ReferenceEquals(existing, entry))
			{
				_ = _byNameChecksum.TryRemove(key, out _);
			}
		}
	}

	public void AddEntry(ObjectIndexEntry entry)
	{
		AddToLookups(entry);
		Objects.Add(entry);
	}

	public void RemoveEntry(ObjectIndexEntry entry)
	{
		RemoveFromLookups(entry);
		_ = Objects.Remove(entry);
	}

	public bool TryFind((string datName, uint datChecksum) key, out ObjectIndexEntry? entry)
	{
		EnsureLookupsBuilt();
		return _byNameChecksum.TryGetValue(key, out entry);
	}

	public bool TryFind(ulong xxHash3, out ObjectIndexEntry? entry)
	{
		EnsureLookupsBuilt();
		return _byXxHash3.TryGetValue(xxHash3, out entry);
	}

	private void EnsureLookupsBuilt()
	{
		if (_byXxHash3.IsEmpty && _byNameChecksum.IsEmpty && Objects.Count > 0)
		{
			RebuildLookups();
		}
	}

	//public bool TryFind(string internalName, out ObjectIndexEntry? entry)
	//{
	//	entry = Objects.FirstOrDefault(x => x.InternalName == internalName);
	//	return entry != null;
	//}

	public async Task SaveIndexAsync(string indexFile)
		=> await JsonFile.SerializeToFileAsync(this, indexFile, JsonFile.DefaultSerializerOptions).ConfigureAwait(false);

	public static async Task<ObjectIndex?> LoadIndexAsync(string indexFile)
		=> await JsonFile.DeserializeFromFileAsync<ObjectIndex?>(indexFile, JsonFile.DefaultSerializerOptions).ConfigureAwait(false);

	// Synchronous wrapper retained for the various console utility apps (DataQuery,
	// DataSanitiser, DatabaseImporter, etc.) which have no SynchronizationContext and
	// cannot meaningfully be made async-from-Main everywhere. Avoid calling this from
	// UI thread code — use LoadOrCreateIndexAsync instead.
	[Obsolete("Use LoadOrCreateIndexAsync to avoid potential deadlocks on UI threads.")]
	public static ObjectIndex LoadOrCreateIndex(string directory, ILogger logger, IProgress<float>? progress = null)
		=> Task.Run(() => LoadOrCreateIndexAsync(directory, logger, progress)).GetAwaiter().GetResult();

	public static async Task<ObjectIndex> LoadOrCreateIndexAsync(string directory, ILogger logger, IProgress<float>? progress = null)
	{
		var indexPath = Path.Combine(directory, DefaultIndexFileName);
		ObjectIndex? index = null;
		if (File.Exists(indexPath))
		{
			logger.LogInformation("Index file found - loading it");
			index = await LoadIndexAsync(indexPath).ConfigureAwait(false);
			index?.RebuildLookups();
		}

		if (index == null)
		{
			logger.LogInformation("Index file not found - creating it");
			index = await CreateIndexAsync(directory, logger, progress).ConfigureAwait(false);
			await index.SaveIndexAsync(indexPath).ConfigureAwait(false);
		}

		return index;
	}

	/// <summary>
	/// Loads the index from <paramref name="indexFile"/> (creating it from disk when it is missing or
	/// malformed), reconciles it with the <c>.dat</c> files under <paramref name="directory"/>, and
	/// persists the result. Both the editor's "reload index" and the server's startup synchronisation
	/// call this so that the two behave identically.
	/// </summary>
	public static async Task<ObjectIndex> LoadOrCreateAndSyncAsync(string directory, string indexFile, ILogger logger, IProgress<float>? progress = null)
	{
		ObjectIndex? index = null;
		if (File.Exists(indexFile))
		{
			try
			{
				index = await LoadIndexAsync(indexFile).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Failed to load index from \"{IndexFile}\"", indexFile);
			}
		}

		var malformed = index?.Objects == null
			|| index.Objects.Any(x => string.IsNullOrEmpty(x.FileName) || string.IsNullOrEmpty(x.DisplayName));

		if (malformed)
		{
			logger.LogWarning("Index file is missing, malformed or its format has changed - recreating it from disk.");
			index = await CreateIndexAsync(directory, logger, progress).ConfigureAwait(false);
		}
		else
		{
			index!.RebuildLookups();
			var (missingEntries, unindexedFiles) = index.DiffWithDisk(directory);
			if (missingEntries.Count > 0 || unindexedFiles.Count > 0)
			{
				logger.LogWarning(
					"Index and files on disk don't match; updating the index now ({Missing} file(s) gone, {New} file(s) new).",
					missingEntries.Count, unindexedFiles.Count);

				index.Delete(e => missingEntries.Contains(e));
				_ = index.UpdateIndex(directory, logger, unindexedFiles, progress);
			}
		}

		await index.SaveIndexAsync(indexFile).ConfigureAwait(false);
		return index;
	}

	/// <summary>
	/// Compares this index against the files on disk: returns the entries whose file is no longer
	/// present and the <c>.dat</c> files that are not indexed. This is the same comparison the editor
	/// performs when it reloads its index, shared so the server's startup sync behaves identically.
	/// </summary>
	public (List<ObjectIndexEntry> MissingEntries, List<string> UnindexedFiles) DiffWithDisk(string directory)
		=> DiffWithDisk(directory, [.. SawyerStreamUtils.GetDatFilesInDirectory(directory)]);

	/// <summary>
	/// Compares this index against <paramref name="files"/> (which must be relative to
	/// <paramref name="directory"/>). The caller supplies the file list so it can apply its own
	/// exclusions - the server, for example, ignores files parked in the <c>Removed</c> folder.
	/// </summary>
	public (List<ObjectIndexEntry> MissingEntries, List<string> UnindexedFiles) DiffWithDisk(string directory, IReadOnlyCollection<string> files)
	{
		List<ObjectIndexEntry> snapshot;
		lock (this)
		{
			snapshot = [.. Objects];
		}

		var fileSet = new HashSet<string>(files, StringComparer.OrdinalIgnoreCase);

		var missing = new List<ObjectIndexEntry>();
		var present = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
		foreach (var entry in snapshot)
		{
			if (string.IsNullOrEmpty(entry.FileName))
			{
				missing.Add(entry);
				continue;
			}

			var relative = MakeRelativeTo(directory, entry.FileName!);
			if (fileSet.Contains(relative))
			{
				_ = present.Add(relative);
			}
			else
			{
				missing.Add(entry);
			}
		}

		var unindexed = files.Where(f => !present.Contains(f)).ToList();
		return (missing, unindexed);
	}

	/// <summary>Returns the index's stored filename relative to <paramref name="directory"/>.</summary>
	public static string MakeRelativeTo(string directory, string fileName)
		=> Path.IsPathRooted(fileName)
			? Path.GetRelativePath(directory, fileName)
			: fileName;

	public static Task<ObjectIndex> CreateIndexAsync(string directory, ILogger logger, IProgress<float>? progress = null)
		=> Task.Run(() => CreateIndex(directory, logger, progress));

	public ObjectIndex UpdateIndex(string directory, ILogger logger, IEnumerable<string> filesToAdd, IProgress<float>? progress = null)
	{
		var (succeeded, failed) = ReadFilesFromDisk(directory, logger, progress, [.. filesToAdd]);

		foreach (var s in succeeded)
		{
			AddEntry(s);
		}

		foreach (var f in failed)
		{
			logger.LogError("Failed to load {F}", f);
		}

		return this;
	}

	public static ObjectIndex CreateIndex(string directory, ILogger logger, IProgress<float>? progress = null)
		=> new ObjectIndex().UpdateIndex(directory, logger, [.. SawyerStreamUtils.GetDatFilesInDirectory(directory)], progress);

	static (ConcurrentQueue<ObjectIndexEntry> succeeded, ConcurrentQueue<string> failed) ReadFilesFromDisk(string directory, ILogger logger, IProgress<float>? progress, string[] files)
	{
		ConcurrentQueue<ObjectIndexEntry> pendingIndices = [];
		ConcurrentQueue<string> failedFiles = [];
		_ = Parallel.ForEach(files, file => ParseFile(directory, file, pendingIndices, failedFiles, files.Length, progress, logger));
		return (pendingIndices, failedFiles);
	}

	public void Delete(Func<ObjectIndexEntry, bool> predicate)
	{
		foreach (var d in Objects.Where(predicate).ToList())
		{
			RemoveEntry(d);
		}
	}

	static void ParseFile(string directory, string filename, ConcurrentQueue<ObjectIndexEntry> pendingIndices, ConcurrentQueue<string> failedFiles, int totalFiles, IProgress<float>? progress, ILogger logger)
	{
		var fullFilename = Path.Combine(directory, filename);

		if (File.Exists(fullFilename))
		{
			ObjectIndexEntry? entry = null;

			try
			{
				var bytes = File.ReadAllBytes(fullFilename);
				entry = GetDatFileInfoFromBytes(fullFilename, filename, bytes, logger);
			}
			catch (IOException ex)
			{
				logger.LogError(ex, "I/O error parsing file \"{Filename}\"", filename);
			}
			catch (UnauthorizedAccessException ex)
			{
				logger.LogError(ex, "Access denied reading file \"{Filename}\"", filename);
			}
			catch (InvalidDataException ex)
			{
				logger.LogError(ex, "Invalid data in file \"{Filename}\"", filename);
			}

			if (entry == null)
			{
				failedFiles.Enqueue(filename);
			}
			else
			{
				pendingIndices.Enqueue(entry);
			}
		}
		else
		{
			failedFiles.Enqueue(filename);
		}

		progress?.Report((pendingIndices.Count + failedFiles.Count) / (float)totalFiles);
	}

	public static ObjectIndexEntry? GetDatFileInfoFromBytes(string absoluteFilename, string relativeFilename, byte[] data, ILogger logger)
	{
		var xxHash3 = XxHash3.HashToUInt64(data);

		if (!SawyerStreamReader.TryGetHeadersFromBytes(data, out var hdrs, logger))
		{
			logger.LogError("{RelativeFilename} must have valid S5 and Object headers to call this method", relativeFilename);
			return null;
		}

		var remainingData = data[(S5Header.StructLength + ObjectHeader.StructLength)..];
		var source = OriginalObjectFiles.GetFileSource(hdrs.S5.Name, hdrs.S5.Checksum, hdrs.S5.ObjectSource);

		var createdTime = DateOnly.FromDateTime(File.GetCreationTimeUtc(absoluteFilename));
		var modifiedTime = DateOnly.FromDateTime(File.GetLastWriteTimeUtc(absoluteFilename));

		var objType = hdrs.S5.ObjectType.Convert();
		if (objType == ObjectType.Vehicle)
		{
			var decoded = SawyerStreamReader.Decode(hdrs.Obj.Encoding, remainingData, 4); // only need 4 bytes since vehicle type is in the 4th byte of a vehicle object
			var vType = (VehicleType)decoded[3];
			return new ObjectIndexEntry(hdrs.S5.Name, relativeFilename, null, hdrs.S5.Checksum, xxHash3, objType, source, createdTime, modifiedTime, vType);
		}
		else
		{
			return new ObjectIndexEntry(hdrs.S5.Name, relativeFilename, null, hdrs.S5.Checksum, xxHash3, objType, source, createdTime, modifiedTime);
		}
	}
}

public record ObjectIndexEntry(
	string DisplayName,
	string? FileName, // only available in local mode
	UniqueObjectId? Id, // only available in online-mode
	uint32_t? DatChecksum,
	ulong? xxHash3,
	ObjectType ObjectType,
	ObjectSource ObjectSource,
	DateOnly? CreatedDate,
	DateOnly? ModifiedDate,
	VehicleType? VehicleType = null)
{
	[JsonIgnore]
	public string SimpleText
		=> $"{DisplayName} | {FileName}";
}
