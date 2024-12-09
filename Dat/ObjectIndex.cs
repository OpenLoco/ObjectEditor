using OpenLoco.Common.Logging;
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Objects;
using OpenLoco.Dat.Types;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace OpenLoco.Dat
{
	public class ObjectIndex : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		IList<ObjectIndexEntry> _objects { get; init; } = [];

		public IReadOnlyList<ObjectIndexEntry> Objects
			=> _objects.AsReadOnly();

		public const string DefaultIndexFileName = "objectIndex.json";

		public ObjectIndex(IList<ObjectIndexEntry> objects)
			=> _objects = objects;

		public bool TryFind((string name, uint checksum) key, out ObjectIndexEntry? entry)
		{
			entry = Objects.FirstOrDefault(x => x.DatName == key.name && x.DatChecksum == key.checksum);
			return entry != null;
		}

		public void SaveIndex(string indexFile)
			=> File.WriteAllText(indexFile, JsonSerializer.Serialize(this));

		public void SaveIndex(string indexFile, JsonSerializerOptions options)
			=> File.WriteAllText(indexFile, JsonSerializer.Serialize(this, options));

		public void Delete(ObjectIndexEntry entry)
		{
			if (_objects.Remove(entry))
			{
				OnPropertyChanged(nameof(Objects));
			}
		}

		public void Delete(Func<ObjectIndexEntry, bool> predicate)
		{
			foreach (var d in _objects.Where(predicate).ToList())
			{
				_ = _objects.Remove(d);
			}
		}

		protected virtual void OnPropertyChanged(string propertyName)
			=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public static async Task<ObjectIndexEntry?> GetDatFileInfoFromBytesAsync((string Filename, byte[] Data) file, ILogger logger)
			=> await Task.Run(() => GetDatFileInfoFromBytes(file, logger)).ConfigureAwait(false);

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
				index = await CreateIndexAsync(directory, logger, progress).ConfigureAwait(false);
				index.SaveIndex(indexPath);
			}

			return index;
		}

		public static ObjectIndex LoadOrCreateIndex(string directory, ILogger logger, IProgress<float>? progress = null)
			=> LoadOrCreateIndexAsync(directory, logger, progress).Result;

		public static async Task<ObjectIndex> CreateIndexAsync(string directory, ILogger logger, IProgress<float>? progress = null)
		{
			var files = SawyerStreamUtils.GetDatFilesInDirectory(directory).ToArray();

			ConcurrentQueue<(string Filename, byte[] Data)> pendingFiles = [];
			ConcurrentQueue<ObjectIndexEntry> pendingIndices = [];
			ConcurrentQueue<string> failedFiles = [];

			var options = new ParallelOptions() { MaxDegreeOfParallelism = 32 };
			var producerTask = Parallel.ForEachAsync(files, options, async (f, ct) => pendingFiles.Enqueue((f, await File.ReadAllBytesAsync(Path.Combine(directory, f), ct).ConfigureAwait(false))));
			var consumerTask = ConsumeInput(pendingIndices, pendingFiles, failedFiles, files.Length, progress, logger);

			await producerTask.ConfigureAwait(false);
			await consumerTask.ConfigureAwait(false);

			await Task.WhenAll(producerTask, consumerTask).ConfigureAwait(false);
			return new ObjectIndex([.. pendingIndices]);
		}

		static async Task ConsumeInput(ConcurrentQueue<ObjectIndexEntry> pendingIndices, ConcurrentQueue<(string Filename, byte[] Data)> pendingFiles, ConcurrentQueue<string> failedFiles, int totalFiles, IProgress<float>? progress, ILogger logger)
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

		public static ObjectIndex CreateIndex(string directory, ILogger logger, IProgress<float>? progress = null)
			=> CreateIndexAsync(directory, logger, progress).Result;

		public static ObjectIndex? LoadIndex(string indexFile)
			=> JsonSerializer.Deserialize<ObjectIndex>(File.ReadAllText(indexFile));

		public static ObjectIndex? LoadIndex(string indexFile, JsonSerializerOptions options)
			=> JsonSerializer.Deserialize<ObjectIndex>(File.ReadAllText(indexFile), options);

		public static async Task<ObjectIndex?> LoadIndexAsync(string indexFile)
		{
			await using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(await File.ReadAllTextAsync(indexFile).ConfigureAwait(false))))
			{
				return await JsonSerializer.DeserializeAsync<ObjectIndex>(stream).ConfigureAwait(false);
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
