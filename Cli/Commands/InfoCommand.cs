using Shared.Files;
using System.Text.Json;

namespace Cli.Commands;

public sealed class InfoCommand : ICommand
{
	public string Name
		=> "info";

	public string Summary
		=> "Print header, string table and image table details for objects";

	public string Usage
		=> "locoobj info <file-or-directory> [--json] [--no-recurse]";

	public IReadOnlySet<string> Options { get; } = new HashSet<string>(CommandContext.CommonOptions, StringComparer.OrdinalIgnoreCase)
	{
		"json",
	};

	public IReadOnlySet<string> Flags { get; } = new HashSet<string>(CommandContext.CommonFlags, StringComparer.OrdinalIgnoreCase)
	{
		"json",
	};

	sealed record ObjectInfo(
		string FileName,
		string Name,
		string ObjectType,
		string ObjectSource,
		string Encoding,
		uint32_t Checksum,
		uint32_t DataLength,
		int ImageCount,
		int ImageGroupCount,
		int StringCount);

	public Task<int> RunAsync(CommandContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		if (!context.TryResolveInputs(out var files, out _))
		{
			return Task.FromResult(ExitCodes.UsageError);
		}

		var asJson = context.Args.Has("json");
		var infos = new List<ObjectInfo>();
		var failed = 0;

		foreach (var fileName in files)
		{
			var file = ObjectFile.Load(fileName, context.Logger);
			if (file == null)
			{
				failed++;
				continue;
			}

			var header = file.DatInfo.S5Header;
			var imageTable = file.LocoObject.ImageTable;

			infos.Add(new ObjectInfo(
				fileName,
				header.Name,
				header.ObjectType.ToString(),
				header.ObjectSource.ToString(),
				file.DatInfo.ObjectHeader.Encoding.ToString(),
				header.Checksum,
				file.DatInfo.ObjectHeader.DataLength,
				imageTable?.Groups.Sum(x => x.GraphicsElements.Count) ?? 0,
				imageTable?.Groups.Count ?? 0,
				file.LocoObject.StringTable.Table.Count));
		}

		if (asJson)
		{
			Console.WriteLine(JsonSerializer.Serialize(infos, new JsonSerializerOptions { WriteIndented = true }));
		}
		else
		{
			foreach (var info in infos)
			{
				Console.WriteLine(info.FileName);
				Console.WriteLine($"  name         {info.Name}");
				Console.WriteLine($"  type         {info.ObjectType}");
				Console.WriteLine($"  source       {info.ObjectSource}");
				Console.WriteLine($"  encoding     {info.Encoding}");
				Console.WriteLine($"  checksum     0x{info.Checksum:X8}");
				Console.WriteLine($"  data length  {info.DataLength}");
				Console.WriteLine($"  images       {info.ImageCount} in {info.ImageGroupCount} group(s)");
				Console.WriteLine($"  strings      {info.StringCount}");
			}
		}

		return Task.FromResult(failed == 0 ? ExitCodes.Success : ExitCodes.OperationFailed);
	}
}
