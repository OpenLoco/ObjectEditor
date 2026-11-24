using Shared.Operations;

namespace Cli.Commands;

public sealed class CropCommand : ICommand
{
	public string Name
		=> "crop";

	public string Summary
		=> "Crop transparent borders off every image, adjusting offsets to match";

	public string Usage
		=> "locoobj crop <file-or-directory> [--out <dir>] [--encoding <enc>] [--palette <png>] [--dry-run] [--no-recurse] [--allow-vanilla]";

	public IReadOnlySet<string> Options
		=> CommandContext.CommonOptions;

	public IReadOnlySet<string> Flags
		=> CommandContext.CommonFlags;

	public Task<int> RunAsync(CommandContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		if (!context.TryResolveInputs(out var files, out var inputRoot))
		{
			return Task.FromResult(ExitCodes.UsageError);
		}

		if (!context.TryBuildBatchOptions(inputRoot, out var options, withPalette: true))
		{
			return Task.FromResult(ExitCodes.UsageError);
		}

		var result = BatchProcessor.Run(
			files,
			file =>
			{
				var cropped = ObjectOperations.CropAllImages(file.LocoObject, context.PaletteMap);
				return cropped == 0
					? OperationOutcome.Unchanged("no images")
					: OperationOutcome.Changed($"cropped {cropped} image(s)");
			},
			options,
			context.Logger);

		return Task.FromResult(CommandContext.Report(result));
	}
}
