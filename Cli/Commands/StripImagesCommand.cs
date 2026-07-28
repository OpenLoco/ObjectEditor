using Core.Operations;

namespace Cli.Commands;

public sealed class StripImagesCommand : ICommand
{
	public string Name
		=> "strip-images";

	public string Summary
		=> "Remove every image from an object's image table";

	public string Usage
		=> "locoobj strip-images <file-or-directory> [--out <dir>] [--encoding <enc>] [--dry-run] [--no-recurse] [--allow-vanilla]";

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

		if (!context.TryBuildBatchOptions(inputRoot, out var options))
		{
			return Task.FromResult(ExitCodes.UsageError);
		}

		var result = BatchProcessor.Run(
			files,
			file =>
			{
				var removed = ObjectOperations.StripImages(file.LocoObject);
				return removed == 0
					? OperationOutcome.Unchanged("no images to strip")
					: OperationOutcome.Changed($"stripped {removed} image(s)");
			},
			options,
			context.Logger);

		return Task.FromResult(context.Report(result));
	}
}
