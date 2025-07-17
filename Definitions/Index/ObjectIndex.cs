using Common.Json;
using Common.Logging;
using Dat.Data;
using Dat.FileParsing;
using Dat.Objects;
using Dat.Types;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO.Hashing;
using System.Text.Json.Serialization;

namespace Definitions.Index;

public class ObjectIndex
{
	public Dictionary<string, ObservableCollection<ObjectIndexEntry>> Indices { get; init; } = [];

	public ObjectIndex()
	{ }

	//public ObjectIndex(string directory, ObservableCollection<ObjectIndexEntry> objects)
	//	=> Indices.Add(directory, objects);

	public ObjectIndex(string directory, IEnumerable<ObjectIndexEntry> objects)
		=> Indices.Add(directory, [.. objects]);

	public ObservableCollection<ObjectIndexEntry>? ObjectsIn(string directory)
		=> Indices.TryGetValue(directory, out var value)
			? value
			: null;

	[JsonIgnore]
	public IEnumerable<ObjectIndexEntry> AllObjects
		=> Indices.SelectMany(x => x.Value);

	public bool TryFind((string datName, uint datChecksum) key, out ObjectIndexEntry? entry)
	{
		entry = AllObjects.FirstOrDefault(x => x.DisplayName == key.datName && x.DatChecksum == key.datChecksum);
		return entry != null;
	}

	public bool TryFind(string directory, (string datName, uint datChecksum) key, out ObjectIndexEntry? entry)
	{
		entry = ObjectsIn(directory)?.FirstOrDefault(x => x.DisplayName == key.datName && x.DatChecksum == key.datChecksum);
		return entry != null;
	}

	public bool TryFind(ulong xxHash3, out ObjectIndexEntry? entry)
	{
		entry = AllObjects.FirstOrDefault(x => x.xxHash3 == xxHash3);
		return entry != null;
	}

	public bool TryFind(string directory, ulong xxHash3, out ObjectIndexEntry? entry)
	{
		entry = ObjectsIn(directory)?.FirstOrDefault(x => x.xxHash3 == xxHash3);
		return entry != null;
	}

	public async Task SaveIndexAsync(string indexFile)
		=> await JsonFile.SerializeToFileAsync(this, indexFile, JsonFile.DefaultSerializerOptions).ConfigureAwait(false);

	public static async Task<ObjectIndex?> LoadIndexAsync(string indexFile)
		=> await JsonFile.DeserializeFromFileAsync<ObjectIndex?>(indexFile, JsonFile.DefaultSerializerOptions).ConfigureAwait(false);

	public static ObjectIndex LoadOrCreateIndex(string indexFile, ILogger logger)
		=> LoadOrCreateIndexAsync(indexFile, logger).Result;

	public static async Task<ObjectIndex> LoadOrCreateIndexAsync(string indexFile, ILogger logger)
	{
		ObjectIndex? index = null;
		if (File.Exists(indexFile))
		{
			logger.Info("Index file found - loading it");

			try
			{
				index = await LoadIndexAsync(indexFile).ConfigureAwait(false);
			}
			catch (Exception ex)
			{
				logger.Error("Index file found but unable to read it. It will be rcreated", ex);
			}
		}

		if (index == null)
		{
			logger.Info("Index file not found - creating it");
			index = new ObjectIndex();
			await index.SaveIndexAsync(indexFile).ConfigureAwait(false);
		}

		return index;
	}

	public async Task<ObservableCollection<ObjectIndexEntry>?> LoadOrCreateIndexDirectoryAsync(string directory, ILogger logger, IProgress<float>? progress = null)
		=> await Task.Run(() => LoadOrCreateIndexDirectory(directory, logger, progress));

	public ObservableCollection<ObjectIndexEntry>? LoadOrCreateIndexDirectory(string directory, ILogger logger, IProgress<float>? progress = null)
	{
		if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory))
		{
			throw new DirectoryNotFoundException(directory);
		}

		if (Indices.TryGetValue(directory, out var existingIndex))
		{
			return existingIndex;
		}
		else
		{
			logger.Info($"Creating index for directory {directory}");

			Indices.Add(directory, []);
			_ = UpdateIndexDirectory(directory, logger, [.. SawyerStreamUtils.GetDatFilesInDirectory(directory)], progress);
		}

		return Indices[directory];
	}

	//public Task<ObjectIndex> CreateIndexAsync(string directory, ILogger logger, IProgress<float>? progress = null)
	//	=> Task.Run(() => CreateIndex(directory, logger, progress));

	public ObjectIndex UpdateIndexDirectory(string directory, ILogger logger, IEnumerable<string> filesToAdd, IProgress<float>? progress = null)
	{
		var (succeeded, failed) = ReadFilesFromDisk(directory, logger, progress, [.. filesToAdd]);
		Indices[directory] = [.. succeeded];

		foreach (var f in failed)
		{
			logger.Error($"Failed to load {f}");
		}

		return this;
	}

	public static ObjectIndex LoadOrCreateIndexFromDirectory(string indexFile, string directory, ILogger logger, IProgress<float>? progress = null)
		=> LoadOrCreateIndex(indexFile, logger)
			.UpdateIndexDirectory(directory, logger, [.. SawyerStreamUtils.GetDatFilesInDirectory(directory)], progress);

	static (ConcurrentQueue<ObjectIndexEntry> succeeded, ConcurrentQueue<string> failed) ReadFilesFromDisk(string directory, ILogger logger, IProgress<float>? progress, string[] files)
	{
		ConcurrentQueue<ObjectIndexEntry> pendingIndices = [];
		ConcurrentQueue<string> failedFiles = [];
		_ = Parallel.ForEach(files, file => ParseFile(directory, file, pendingIndices, failedFiles, files.Length, progress, logger));
		return (pendingIndices, failedFiles);
	}

	static void ParseFile(string directory, string filename, ConcurrentQueue<ObjectIndexEntry> pendingIndices, ConcurrentQueue<string> failedFiles, int totalFiles, IProgress<float>? progress, ILogger logger)
	{
		var fullFilename = Path.Combine(directory, filename);

		if (File.Exists(fullFilename))
		{
			var bytes = File.ReadAllBytes(fullFilename);
			ObjectIndexEntry? entry = null;

			try
			{
				entry = GetDatFileInfoFromBytes(fullFilename, filename, bytes, logger);
			}
			catch (Exception ex)
			{
				logger.Error(ex);
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

	static ObjectIndexEntry? GetDatFileInfoFromBytes(string absoluteFilename, string relativeFilename, byte[] data, ILogger logger)
	{
		var xxHash3 = XxHash3.HashToUInt64(data);

		if (!SawyerStreamReader.TryGetHeadersFromBytes(data, out var hdrs, logger))
		{
			logger.Error($"{relativeFilename} must have valid S5 and Object headers to call this method", nameof(relativeFilename));
			return null;
		}

		var remainingData = data[(S5Header.StructLength + ObjectHeader.StructLength)..];
		var source = OriginalObjectFiles.GetFileSource(hdrs.S5.Name, hdrs.S5.Checksum);

		var createdTime = DateOnly.FromDateTime(File.GetCreationTimeUtc(absoluteFilename));
		var modifiedTime = DateOnly.FromDateTime(File.GetLastWriteTimeUtc(absoluteFilename));

		if (hdrs.S5.ObjectType == ObjectType.Vehicle)
		{
			var decoded = SawyerStreamReader.Decode(hdrs.Obj.Encoding, remainingData, 4); // only need 4 bytes since vehicle type is in the 4th byte of a vehicle object
			var vType = (VehicleType)decoded[3];
			return new ObjectIndexEntry(hdrs.S5.Name, relativeFilename, null, hdrs.S5.Checksum, xxHash3, hdrs.S5.ObjectType, source, createdTime, modifiedTime, vType);
		}
		else
		{
			return new ObjectIndexEntry(hdrs.S5.Name, relativeFilename, null, hdrs.S5.Checksum, xxHash3, hdrs.S5.ObjectType, source, createdTime, modifiedTime);
		}
	}
}
