// See https://aka.ms/new-console-template for more information
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;
using OpenLoco.ObjectEditor.Objects;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

Console.WriteLine("Hello, World!");

var files = Directory.GetFiles("Q:\\Games\\Locomotion\\Glens Folder\\Loco Dat Vault Aug2024", "*.dat", SearchOption.AllDirectories);
Console.WriteLine(files.Length);

var sw = new Stopwatch();
sw.Start();

//var datTask1 = await FastIndex(files);
//Console.WriteLine($"(1) {sw.Elapsed} {datTask1.Count}");
//sw.Restart();

//var datTask2 = await FasterIndex(files);
//Console.WriteLine($"(2) {sw.Elapsed} {datTask2.Count}");
//sw.Restart();

var datTask3 = await FastestIndex(files);
Console.WriteLine($"(3) {sw.Elapsed} {datTask3.Count}");
sw.Restart();

sw.Stop();

Console.WriteLine("done");
Console.ReadLine();

static async Task<List<ObjectIndex>> FastIndex(string[] files)
{
	ConcurrentQueue<byte[]> pendingFiles = [];
	ConcurrentQueue<ObjectIndex> pendingDats = [];

	// this loads all files into a pending queue
	var options = new ParallelOptions() { MaxDegreeOfParallelism = 32 };
	await Parallel.ForEachAsync(files, options, async (f, ct) =>
	{
		var bytes = await File.ReadAllBytesAsync(f, ct);
		if (bytes.Length < 21)
		{
			Console.WriteLine($"file {f} has length {bytes.Length}");
		}
		else
		{
			pendingFiles.Enqueue(bytes);
		}
	});

	Console.WriteLine("between");

	// now process the queue
	await Process(pendingFiles, pendingDats, files);

	return [.. pendingDats];

	static async Task Process(ConcurrentQueue<byte[]> pendingFiles, ConcurrentQueue<ObjectIndex> pendingDats, string[] files)
	{
		var processed = 0;
		while (processed < files.Length)
		{
			if (pendingFiles.TryPeek(out var file))
			{
				pendingDats.Enqueue(await GetDatFileInfoFromBytesFast(file));
			}
			_ = Interlocked.Increment(ref processed);

			if (processed % 1000 == 0)
			{
				//Console.WriteLine(processed);
			}
		}
	}
}
static async Task<List<ObjectIndex>> FastestIndex(string[] files)
{
	ConcurrentQueue<byte[]> pendingFiles = [];
	ConcurrentQueue<ObjectIndex> pendingDats = [];

	var producerTask = Task.Run(async () =>
	{
		// act
		var options = new ParallelOptions() { MaxDegreeOfParallelism = 32 };
		await Parallel.ForEachAsync(files, options, async (f, ct) => pendingFiles.Enqueue(await File.ReadAllBytesAsync(f, ct)));
	});

	// Consumer: Process file content from the buffer
	var consumerTask = Task.Run(async () =>
	{
		while (!producerTask.IsCompleted && pendingDats.Count != files.Length)
		{
			if (pendingFiles.TryPeek(out var bytes) && bytes!.Length >= (S5Header.StructLength + ObjectHeader.StructLength))
			{
				pendingDats.Enqueue(await GetDatFileInfoFromBytesFast(bytes));
			}
		}
	});

	// Wait for both producer and consumer to finish
	await Task.WhenAll(producerTask, consumerTask);

	return [.. pendingDats];
}

static async Task<List<ObjectIndex>> FasterIndex(IEnumerable<string> files)
{
	// arrange
	ConcurrentQueue<ObjectIndex> pendingDats = [];
	var bufferBlock = new BufferBlock<byte[]>();
	var actionBlock = new ActionBlock<byte[]>(async bytes =>
	{
		//var fileContent = await File.ReadAllBytesAsync(filePath);
		//if (bytes.Length > (S5Header.StructLength + ObjectHeader.StructLength))
		{
			pendingDats.Enqueue(await GetDatFileInfoFromBytesFast(bytes));
		}

	}, new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 32 }); // Adjust parallelism as needed

	_ = bufferBlock.LinkTo(actionBlock, new DataflowLinkOptions { PropagateCompletion = true });

	// act
	var options = new ParallelOptions() { MaxDegreeOfParallelism = 32 };
	await Parallel.ForEachAsync(files, options, async (f, ct) => await bufferBlock.SendAsync(await File.ReadAllBytesAsync(f, ct)));

	// production complete
	bufferBlock.Complete();

	// wait
	await actionBlock.Completion;
	return [.. pendingDats];
}

static async Task<ObjectIndex> GetDatFileInfoFromBytesFast(byte[] data)
	=> await Task.Run<ObjectIndex>(() =>
	{
		var span = data.AsSpan();
		var s5 = S5Header.Read(span[0..S5Header.StructLength]);
		var oh = ObjectHeader.Read(span[S5Header.StructLength..(S5Header.StructLength + ObjectHeader.StructLength)]);
		var remainingData = span[(S5Header.StructLength + ObjectHeader.StructLength)..];
		if (s5.ObjectType == ObjectType.Vehicle)
		{
			var decoded = SawyerStreamReader.Decode(oh.Encoding, remainingData, 4); // only need 
			return new(s5, oh, (VehicleType)decoded[3]); // 3rd byte is vehicle type
		}
		else
		{
			return new(s5, oh);
		}
	});

static async Task<ObjectIndex> GetDatFileInfoFromBytesSlow(byte[] data)
	=> await Task.Run<ObjectIndex>(() =>
	{
		var (d, l) = SawyerStreamReader.LoadFullObjectFromStream(data);
		var s5 = d.S5Header;
		var oh = d.ObjectHeader;

		if (s5.ObjectType == ObjectType.Vehicle)
		{
			return new(s5, oh, (l!.Object as VehicleObject)!.Type);
		}
		else
		{
			return new(s5, oh);
		}
	});

public record ObjectIndex(S5Header s5, ObjectHeader oh, VehicleType? VehicleType = null);
