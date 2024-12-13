using Common.Json;
using OpenLoco.Common.Logging;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OpenLoco.Dat
{
	public class ObjectIndex
	{
		public List<ObjectIndexEntry> Objects { get; init; } = [];


		[JsonIgnore]
		public const string DefaultIndexFileName = "objectIndex.json";

		[JsonIgnore]
		static JsonSerializerOptions JsonOptions { get; } = new() { WriteIndented = true, AllowTrailingCommas = true };

		public ObjectIndex()
		{ }

		public ObjectIndex(List<ObjectIndexEntry> objects)
			=> Objects = objects;

		public bool TryFind((string name, uint checksum) key, out ObjectIndexEntry? entry)
		{
			entry = Objects.FirstOrDefault(x => x.DatName == key.name && x.DatChecksum == key.checksum);
			return entry != null;
		}

		public async Task SaveIndexAsync(string indexFile)
			=> await JsonFile.SerializeToFileAsync(this, indexFile, JsonOptions).ConfigureAwait(false);

		public static async Task<ObjectIndex?> LoadIndexAsync(string indexFile)
			=> await JsonFile.DeserializeFromFileAsync<ObjectIndex?>(indexFile, JsonOptions).ConfigureAwait(false);

		public static ObjectIndex LoadOrCreateIndex(string directory, ILogger logger, IProgress<float>? progress = null)
			=> LoadOrCreateIndexAsync(directory, logger, progress).Result;

		public static async Task<ObjectIndex> LoadOrCreateIndexAsync(string directory, ILogger logger, IProgress<float>? progress = null)
		{
			var indexPath = Path.Combine(directory, DefaultIndexFileName);
			ObjectIndex? index = null;
			if (File.Exists(indexPath))
			{
				index = await LoadIndexAsync(indexPath).ConfigureAwait(false);
			}

			if (index == null)
			{
				index = await CreateIndexAsync(directory, logger, progress).ConfigureAwait(false);
				await index.SaveIndexAsync(indexPath).ConfigureAwait(false);
			}

			return index;
		}

		public static ObjectIndex CreateIndex(string directory, ILogger logger, IProgress<float>? progress = null)
			=> CreateIndexAsync(directory, logger, progress).Result;

		public static async Task<ObjectIndex> CreateIndexAsync(string directory, ILogger logger, IProgress<float>? progress = null)
		{
			var files = SawyerStreamUtils.GetDatFilesInDirectory(directory).ToArray();

			ConcurrentQueue<(string Filename, byte[] Data)> pendingFiles = [];
			ConcurrentQueue<ObjectIndexEntry> pendingIndices = [];
			ConcurrentQueue<string> failedFiles = [];

			var options = new ParallelOptions() { MaxDegreeOfParallelism = 32 };

			var producerTask = Parallel.ForEachAsync(files, options, async (f, ct)
				=> pendingFiles.Enqueue((f, await File.ReadAllBytesAsync(Path.Combine(directory, f), ct).ConfigureAwait(false))));

			var consumerTask = ConsumeInputAsync(pendingIndices, pendingFiles, failedFiles, files.Length, progress, logger);

			await producerTask.ConfigureAwait(false);
			await consumerTask.ConfigureAwait(false);
			await Task.WhenAll(producerTask, consumerTask).ConfigureAwait(false);

			return new ObjectIndex([.. pendingIndices]);
		}

		public void Delete(Func<ObjectIndexEntry, bool> predicate)
		{
			foreach (var d in Objects.Where(predicate).ToList())
			{
				_ = Objects.Remove(d);
			}
		}

		public static async Task<ObjectIndexEntry?> GetDatFileInfoFromBytesAsync((string Filename, byte[] Data) file, ILogger logger)
			=> await Task.Run(() => GetDatFileInfoFromBytes(file, logger)).ConfigureAwait(false);

		static async Task ConsumeInputAsync(ConcurrentQueue<ObjectIndexEntry> pendingIndices, ConcurrentQueue<(string Filename, byte[] Data)> pendingFiles, ConcurrentQueue<string> failedFiles, int totalFiles, IProgress<float>? progress, ILogger logger)
		{
			while ((pendingIndices.Count + failedFiles.Count) != totalFiles)
			{
				if (pendingFiles.TryDequeue(out var content))
				{
					ObjectIndexEntry? entry = null;
#pragma warning disable RCS1075 // Avoid empty catch clause that catches System.Exception. This is fine because on Exception,  `entry` will be null and that is handled afterwards
					try
					{
						entry = await GetDatFileInfoFromBytesAsync(content, logger).ConfigureAwait(false);
					}
					catch (Exception)
					{ }
#pragma warning restore RCS1075 // Avoid empty catch clause that catches System.Exception

					if (entry == null)
					{
						failedFiles.Enqueue(content.Filename);
					}
					else
					{
						pendingIndices.Enqueue(entry);
					}

					progress?.Report((pendingIndices.Count + failedFiles.Count) / (float)totalFiles);
				}
			}
		}

		public static ObjectIndexEntry? GetDatFileInfoFromBytes((string Filename, byte[] Data) file, ILogger logger)
		{
			if (!SawyerStreamReader.TryGetHeadersFromBytes(file.Data, out var hdrs, logger))
			{
				logger.Error($"{file.Filename} must have valid S5 and Object headers to call this method", nameof(file));
				return null;
			}

			var remainingData = file.Data[(S5Header.StructLength + ObjectHeader.StructLength)..];
			var source = OriginalObjectFiles.GetFileSource(hdrs.S5.Name, hdrs.S5.Checksum);

			if (hdrs.S5.ObjectType == ObjectType.Vehicle)
			{
				var decoded = SawyerStreamReader.Decode(hdrs.Obj.Encoding, remainingData, 4); // only need 4 bytes since vehicle type is in the 4th byte of a vehicle object
				return new ObjectIndexEntry(file.Filename, hdrs.S5.Name, hdrs.S5.Checksum, hdrs.S5.ObjectType, source, (VehicleType)decoded[3]);
			}
			else
			{
				return new ObjectIndexEntry(file.Filename, hdrs.S5.Name, hdrs.S5.Checksum, hdrs.S5.ObjectType, source);
			}
		}
	}

	public record ObjectIndexEntry(
		string Filename,
		string DatName,
		uint32_t DatChecksum,
		ObjectType ObjectType,
		ObjectSource ObjectSource,
		VehicleType? VehicleType = null);
}
