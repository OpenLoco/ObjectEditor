using Core.Graphics;
using Core.Objects;
using Microsoft.Extensions.Logging;

namespace Cli.Commands;

public sealed class ImportImagesCommand : ICommand
{
	public string Name
		=> "import-images";

	public string Summary
		=> "Replace an object's image table from a directory of PNGs and a sprites.json";

	public string Usage
		=> "locoobj import-images <file.dat> --from <dir> [--out <file.dat>] [--encoding <enc>] [--palette <png>] [--offsets-only] [--dry-run] [--allow-vanilla]";

	public IReadOnlySet<string> Options { get; } = new HashSet<string>(CommandContext.CommonOptions, StringComparer.OrdinalIgnoreCase)
	{
		"from", "offsets-only",
	};

	public IReadOnlySet<string> Flags { get; } = new HashSet<string>(CommandContext.CommonFlags, StringComparer.OrdinalIgnoreCase)
	{
		"offsets-only",
	};

	public async Task<int> RunAsync(CommandContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		var source = context.Args.GetString("from");
		if (string.IsNullOrEmpty(source))
		{
			context.Logger.LogError("--from <dir> is required");
			return ExitCodes.UsageError;
		}

		var inputFile = context.Args.Positionals.Count > 0 ? context.Args.Positionals[0] : null;
		if (string.IsNullOrEmpty(inputFile) || !File.Exists(inputFile))
		{
			context.Logger.LogError("A single existing .dat file must be given as the first argument");
			return ExitCodes.UsageError;
		}

		if (!context.TryGetEncoding(out var encoding))
		{
			return ExitCodes.UsageError;
		}

		var file = ObjectFile.Load(inputFile, context.Logger, context.PaletteMap);
		if (file?.LocoObject.ImageTable == null)
		{
			context.Logger.LogError("\"{FileName}\" has no image table", inputFile);
			return ExitCodes.OperationFailed;
		}

		var imageTable = file.LocoObject.ImageTable;
		int count;

		if (context.Args.Has("offsets-only"))
		{
			var spritesFile = Directory.Exists(source) ? Path.Combine(source, ImageTableIo.SpritesFileName) : source;
			count = await ImageTableIo.ApplyOffsetsAsync(imageTable, spritesFile, context.Logger);
		}
		else
		{
			count = await ImageTableIo.ImportAsync(imageTable, source, context.PaletteMap, context.Logger, file.LocoObject.Object, file.LocoObject.ObjectType);
		}

		if (count == 0)
		{
			context.Logger.LogError("Nothing was imported from \"{Source}\"", source);
			return ExitCodes.OperationFailed;
		}

		var outputFile = context.OutputPath ?? inputFile;

		if (context.DryRun)
		{
			Console.WriteLine($"ok   {inputFile}: imported {count} image(s) (dry run, would write \"{outputFile}\")");
			return ExitCodes.Success;
		}

		if (!ObjectFile.SaveDat(file, outputFile, context.Logger, encoding, allowSavingAsVanillaObject: context.AllowSavingAsVanillaObject))
		{
			return ExitCodes.OperationFailed;
		}

		Console.WriteLine($"ok   {inputFile}: imported {count} image(s) into \"{outputFile}\"");
		return ExitCodes.Success;
	}
}
