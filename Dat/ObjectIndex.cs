using OpenLoco.Common.Logging;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace OpenLoco.Dat
{
	public class ObjectIndex
	{
		public required IList<ObjectIndexEntry> Objects { get; init; } = [];

		public const string DefaultIndexFileName = "objectIndex.json";

		public bool TryFind((string name, uint checksum) key, out ObjectIndexEntry? entry)
		{
			entry = Objects.FirstOrDefault(x => x.DatName == key.name && x.DatChecksum == key.checksum);
			return entry != null;
		}

		public void SaveIndex(string indexFile)
			=> File.WriteAllText(indexFile, JsonSerializer.Serialize(this));

		public void SaveIndex(string indexFile, JsonSerializerOptions options)
			=> File.WriteAllText(indexFile, JsonSerializer.Serialize(this, options));

		public static async Task<ObjectIndexEntry> GetDatFileInfoFromBytesAsync((string Filename, byte[] Data) file, ILogger logger)
			=> await Task.Run(() => GetDatFileInfoFromBytes(file, logger));

		public static async Task<ObjectIndex> LoadOrCreateIndexAsync(string directory, ILogger logger, IProgress<float>? progress = null)
		{
			var indexPath = Path.Combine(directory, DefaultIndexFileName);
			ObjectIndex? index = null;
			if (File.Exists(indexPath))
			{
				index = LoadIndex(indexPath);
			}

			if (index == null)
			{
				index = await CreateIndexAsync(directory, logger, progress);
				index.SaveIndex(indexPath);
			}

			return index;
		}

		public static ObjectIndex LoadOrCreateIndex(string directory, ILogger logger, IProgress<float>? progress = null)
			=> LoadOrCreateIndexAsync(directory, logger, progress).Result;

		public static Task<ObjectIndex> CreateIndexAsync(string directory, ILogger logger, IProgress<float>? progress = null)
		{
			var files = SawyerStreamUtils.GetDatFilesInDirectory(directory).ToArray();

			ConcurrentQueue<(string Filename, byte[] Data)> pendingFiles = [];
			ConcurrentQueue<ObjectIndexEntry> pendingIndices = [];

			var producerTask = Task.Run(async () =>
			{
				var options = new ParallelOptions() { MaxDegreeOfParallelism = 32 };
				await Parallel.ForEachAsync(files, options, async (f, ct) => pendingFiles.Enqueue((f, await File.ReadAllBytesAsync(Path.Combine(directory, f), ct))));
			});

			var consumerTask = Task.Run(async () =>
			{
				while (pendingIndices.Count != files.Length)
				{
					if (pendingFiles.TryDequeue(out var content))
					{
						pendingIndices.Enqueue(await GetDatFileInfoFromBytesAsync(content, logger)); // no possible way to know if an object is invalid from partial analysis, so this will always return 'valid'
						progress?.Report(pendingIndices.Count / (float)files.Length);
					}
				}
			});

			return Task.Run(async () =>
			{
				await Task.WhenAll(producerTask, consumerTask);
				return new ObjectIndex() { Objects = [.. pendingIndices] };
			});
		}

		public static ObjectIndex CreateIndex(string directory, ILogger logger, IProgress<float>? progress = null)
			=> CreateIndexAsync(directory, logger, progress).Result;

		public static ObjectIndex? LoadIndex(string indexFile)
			=> JsonSerializer.Deserialize<ObjectIndex>(File.ReadAllText(indexFile));

		public static ObjectIndex? LoadIndex(string indexFile, JsonSerializerOptions options)
			=> JsonSerializer.Deserialize<ObjectIndex>(File.ReadAllText(indexFile), options);

		public static async Task<ObjectIndex?> LoadIndexAsync(string indexFile)
		{
			await using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(await File.ReadAllTextAsync(indexFile))))
			{
				return await JsonSerializer.DeserializeAsync<ObjectIndex>(stream);
			}
		}

		public static ObjectIndexEntry GetDatFileInfoFromBytes((string Filename, byte[] Data) file, ILogger logger)
		{
			if (!SawyerStreamReader.TryGetHeadersFromBytes(file.Data, out var hdrs, logger))
			{
				throw new ArgumentException($"{file.Filename} must have valid S5 and Object headers to call this method", nameof(file));
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
