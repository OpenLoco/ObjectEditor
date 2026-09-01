using Microsoft.Extensions.Logging;
using Shared.Operations;

namespace Cli.Commands;

public sealed class OffsetsCommand : ICommand
{
	public string Name
		=> "offsets";

	public string Summary
		=> "Bulk-edit the x/y offsets of every image in an object";

	public string Usage
		=> "locoobj offsets <file-or-directory> (--zero | --center | --translate <x,y>) [--out <dir>] [--encoding <enc>] [--dry-run] [--no-recurse] [--allow-vanilla]";

	public IReadOnlySet<string> Options { get; } = new HashSet<string>(CommandContext.CommonOptions, StringComparer.OrdinalIgnoreCase)
	{
		"zero", "center", "translate",
	};

	public IReadOnlySet<string> Flags { get; } = new HashSet<string>(CommandContext.CommonFlags, StringComparer.OrdinalIgnoreCase)
	{
		"zero", "center",
	};

	public Task<int> RunAsync(CommandContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		var zero = context.Args.Has("zero");
		var center = context.Args.Has("center");
		var translate = context.Args.GetString("translate");

		var modeCount = (zero ? 1 : 0) + (center ? 1 : 0) + (translate != null ? 1 : 0);
		if (modeCount != 1)
		{
			context.Logger.LogError("Exactly one of --zero, --center or --translate <x,y> must be given");
			return Task.FromResult(ExitCodes.UsageError);
		}

		short deltaX = 0;
		short deltaY = 0;

		if (translate != null && !TryParseDelta(translate, out deltaX, out deltaY))
		{
			context.Logger.LogError("--translate expects two comma-separated whole numbers, for example --translate 4,-2");
			return Task.FromResult(ExitCodes.UsageError);
		}

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
				var count = zero
					? ObjectOperations.ZeroAllOffsets(file.LocoObject)
					: center
						? ObjectOperations.CenterAllOffsets(file.LocoObject)
						: ObjectOperations.TranslateAllOffsets(file.LocoObject, deltaX, deltaY);

				return count == 0
					? OperationOutcome.Unchanged("no images")
					: OperationOutcome.Changed($"updated offsets on {count} image(s)");
			},
			options,
			context.Logger);

		return Task.FromResult(CommandContext.Report(result));
	}

	static bool TryParseDelta(string value, out short deltaX, out short deltaY)
	{
		deltaX = 0;
		deltaY = 0;

		var parts = value.Split(',', StringSplitOptions.TrimEntries);
		return parts.Length == 2
			&& short.TryParse(parts[0], out deltaX)
			&& short.TryParse(parts[1], out deltaY);
	}
}
