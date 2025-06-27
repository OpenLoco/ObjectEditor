using Common.Json;
using OpenLoco.Common.Logging;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO.Hashing;
using System.Text.Json.Serialization;

namespace OpenLoco.Definitions.Database
{
	public class ObjectIndex
	{
		public ObservableCollection<ObjectIndexEntry> Objects { get; init; } = [];

		[JsonIgnore]
		public const string DefaultIndexFileName = "objectIndex.json";

		[JsonIgnore]
		public const string DefaultIndexDbFileName = "objectIndex.db";

		public ObjectIndex()
		{ }

		public ObjectIndex(ObservableCollection<ObjectIndexEntry> objects)
			=> Objects = objects;

		public ObjectIndex(IEnumerable<ObjectIndexEntry> objects)
			=> Objects = [.. objects];

		public bool TryFind((string datName, uint datChecksum) key, out ObjectIndexEntry? entry)
		{
			entry = Objects.FirstOrDefault(x => x.DisplayName == key.datName && x.DatChecksum == key.datChecksum);
			return entry != null;
		}

		public bool TryFind(ulong xxHash3, out ObjectIndexEntry? entry)
		{
			entry = Objects.FirstOrDefault(x => x.xxHash3 == xxHash3);
			return entry != null;
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

		public static ObjectIndex LoadOrCreateIndex(string directory, ILogger logger, IProgress<float>? progress = null)
			=> LoadOrCreateIndexAsync(directory, logger, progress).Result;

		public static async Task<ObjectIndex> LoadOrCreateIndexAsync(string directory, ILogger logger, IProgress<float>? progress = null)
		{
			var indexPath = Path.Combine(directory, DefaultIndexFileName);
			ObjectIndex? index = null;
			if (File.Exists(indexPath))
			{
				logger.Info("Index file found - loading it");
				index = await LoadIndexAsync(indexPath).ConfigureAwait(false);
			}

			if (index == null)
			{
				logger.Info("Index file not found - creating it");
				index = await CreateIndexAsync(directory, logger, progress).ConfigureAwait(false);
				await index.SaveIndexAsync(indexPath).ConfigureAwait(false);
			}

			return index;
		}

		public static Task<ObjectIndex> CreateIndexAsync(string directory, ILogger logger, IProgress<float>? progress = null)
			=> Task.Run(() => CreateIndex(directory, logger, progress));

		public ObjectIndex UpdateIndex(string directory, ILogger logger, IEnumerable<string> filesToAdd, IProgress<float>? progress = null)
		{
			var (succeeded, failed) = ReadFilesFromDisk(directory, logger, progress, [.. filesToAdd]);

			foreach (var s in succeeded)
			{
				Objects.Add(s);
			}

			foreach (var f in failed)
			{
				logger.Error($"Failed to load {f}");
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
				_ = Objects.Remove(d);
			}
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

		public static ObjectIndexEntry? GetDatFileInfoFromBytes(string absoluteFilename, string relativeFilename, byte[] data, ILogger logger)
		{
			var xxHash3 = XxHash3.HashToUInt64(data);

			if (!SawyerStreamReader.TryGetHeadersFromBytes(data, out var hdrs, logger))
			{
				logger.Error($"{relativeFilename} must have valid S5 and Object headers to call this method", nameof(relativeFilename));
				return null;
			}

			var remainingData = data[(S5Header.StructLength + ObjectHeader.StructLength)..];
			var source = OriginalObjectFiles.GetFileSource(hdrs.S5.Name, hdrs.S5.Checksum);

			var createdTime = File.GetCreationTimeUtc(absoluteFilename);
			var modifiedTime = File.GetLastWriteTimeUtc(absoluteFilename);

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

	public record ObjectIndexEntry(
		string DisplayName,
		string? FileName, // only available in local mode
		UniqueObjectId? Id, // only available in online-mode
		uint32_t? DatChecksum,
		ulong? xxHash3,
		ObjectType ObjectType,
		ObjectSource ObjectSource,
		DateTimeOffset? CreatedDate,
		DateTimeOffset? ModifiedDate,
		VehicleType? VehicleType = null)
	{
		[JsonIgnore]
		public string SimpleText
			=> $"{DisplayName} | {FileName}";
	}
}
