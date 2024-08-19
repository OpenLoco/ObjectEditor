using OpenLoco.Dat.Data;
using OpenLoco.Dat.Objects;
using System.Collections.Concurrent;

namespace OpenLoco.Dat.FileParsing
{
	public class ObjectIndex
	{
		public const int JsonVersion = 1; // change this every time this format changes
		public int Version => JsonVersion;

		public required IEnumerable<ObjectIndexEntry> Objects { get; set; }

		public required IEnumerable<ObjectIndexFailedEntry> ObjectsFailed { get; set; }

		public static Task<ObjectIndex> FastIndexAsync(string[] files, IProgress<float> progress)
		{
			ConcurrentQueue<(string Filename, byte[] Data)> pendingFiles = [];
			ConcurrentQueue<ObjectIndexEntryBase> pendingIndices = [];

			var producerTask = Task.Run(async () =>
			{
				var options = new ParallelOptions() { MaxDegreeOfParallelism = 32 };
				await Parallel.ForEachAsync(files, options, async (f, ct) => pendingFiles.Enqueue((f, await File.ReadAllBytesAsync(f, ct))));
			});

			var consumerTask = Task.Run(async () =>
			{
				while (pendingIndices.Count != files.Length)
				{
					if (pendingFiles.TryDequeue(out var content))
					{
						pendingIndices.Enqueue(await SawyerStreamReader.GetDatFileInfoFromBytes(content));
						progress.Report(pendingIndices.Count / (float)files.Length);
					}
				}
			});

			return Task.Run(async () =>
			{
				await Task.WhenAll(producerTask, consumerTask);
				return new ObjectIndex() { Objects = pendingIndices.OfType<ObjectIndexEntry>(), ObjectsFailed = pendingIndices.OfType<ObjectIndexFailedEntry>() };
			});
		}
	}

	public abstract record ObjectIndexEntryBase(string Filename);

	public record ObjectIndexEntry(string Filename, string ObjectName, ObjectType ObjectType, SourceGame SourceGame, uint32_t Checksum, VehicleType? VehicleType = null)
		: ObjectIndexEntryBase(Filename);

	public record ObjectIndexFailedEntry(string Filename)
		: ObjectIndexEntryBase(Filename);
}
