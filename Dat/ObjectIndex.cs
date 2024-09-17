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

		public required IList<ObjectIndexFailedEntry> ObjectsFailed { get; init; } = [];

		public void AddObject(ObjectIndexEntryBase entryBase)
		{
			if (entryBase is ObjectIndexEntry entry)
			{
				Objects.Add(entry);
			}
			else if (entryBase is ObjectIndexFailedEntry failed)
			{
				ObjectsFailed.Add(failed);
			}
		}

		public bool TryFind((string name, uint checksum) key, out ObjectIndexEntry? entry)
		{
			entry = Objects.FirstOrDefault(x => x.ObjectName == key.name && x.Checksum == key.checksum);
			return entry != null;
		}

		public static async Task<ObjectIndex?> LoadOrCreateIndexAsync(string directory, IProgress<float>? progress = null)
		{
			var indexPath = Path.Combine(directory, "objectIndex.json");
			ObjectIndex? index;
			if (File.Exists(indexPath))
			{
				index = LoadIndex(indexPath);
			}
			else
			{
				index = await CreateIndexAsync(directory, progress);
				index.SaveIndex(indexPath);
			}

			return index;
		}

		public static ObjectIndex? LoadOrCreateIndex(string directory, IProgress<float>? progress = null)
			=> LoadOrCreateIndexAsync(directory, progress).Result;

		public static Task<ObjectIndex> CreateIndexAsync(string directory, IProgress<float>? progress = null)
		{
			var files = SawyerStreamUtils.GetDatFilesInDirectory(directory).ToArray();

			ConcurrentQueue<(string Filename, byte[] Data)> pendingFiles = [];
			ConcurrentQueue<ObjectIndexEntryBase> pendingIndices = [];

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
						pendingIndices.Enqueue(await GetDatFileInfoFromBytesAsync(content));
						progress?.Report(pendingIndices.Count / (float)files.Length);
					}
				}
			});

			return Task.Run(async () =>
			{
				await Task.WhenAll(producerTask, consumerTask);
				return new ObjectIndex() { Objects = pendingIndices.OfType<ObjectIndexEntry>().ToList(), ObjectsFailed = pendingIndices.OfType<ObjectIndexFailedEntry>().ToList() };
			});
		}

		public static ObjectIndex CreateIndex(string directory, IProgress<float>? progress = null)
			=> CreateIndexAsync(directory, progress).Result;

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

		public void SaveIndex(string indexFile)
			=> File.WriteAllText(indexFile, JsonSerializer.Serialize(this));

		public void SaveIndex(string indexFile, JsonSerializerOptions options)
			=> File.WriteAllText(indexFile, JsonSerializer.Serialize(this, options));

		public static async Task<ObjectIndexEntryBase> GetDatFileInfoFromBytesAsync((string Filename, byte[] Data) file)
			=> await Task.Run((Func<ObjectIndexEntryBase>)(() =>
			{
				if (file.Data!.Length < (S5Header.StructLength + ObjectHeader.StructLength))
				{
					return new ObjectIndexFailedEntry(file.Filename);
				}

				var span = file.Data.AsSpan();
				var s5 = S5Header.Read(span[0..S5Header.StructLength]);
				var oh = ObjectHeader.Read(span[S5Header.StructLength..(S5Header.StructLength + ObjectHeader.StructLength)]);

				if (!s5.IsValid() || !oh.IsValid())
				{
					return new ObjectIndexFailedEntry(file.Filename);
				}

				var remainingData = span[(S5Header.StructLength + ObjectHeader.StructLength)..];
				var isVanilla = s5.IsOriginal();

				if (s5.ObjectType == ObjectType.Vehicle)
				{
					var decoded = SawyerStreamReader.Decode(oh.Encoding, remainingData, 4); // only need 4 bytes since vehicle type is in the 4th byte of a vehicle object
					return new ObjectIndexEntry(file.Filename, s5.Name, s5.ObjectType, isVanilla, s5.Checksum, (VehicleType)decoded[3]);
				}
				else
				{
					return new ObjectIndexEntry(file.Filename, s5.Name, s5.ObjectType, isVanilla, s5.Checksum);
				}
			}));
	}

	public abstract record ObjectIndexEntryBase(string Filename);

	public record ObjectIndexEntry(
		string Filename,
		string ObjectName,
		ObjectType ObjectType,
		bool IsVanilla,
		uint32_t Checksum,
		VehicleType? VehicleType = null)
		: ObjectIndexEntryBase(Filename);

	public record ObjectIndexFailedEntry(string Filename)
		: ObjectIndexEntryBase(Filename);
}
