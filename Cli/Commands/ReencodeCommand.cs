using Microsoft.Extensions.Logging;
using Shared.Operations;

namespace Cli.Commands;

public sealed class ReencodeCommand : ICommand
{
	public string Name
		=> "reencode";

	public string Summary
		=> "Rewrite objects using a different Sawyer encoding";

	public string Usage
		=> "locoobj reencode <file-or-directory> --encoding <Uncompressed|RunLengthSingle|RunLengthMulti|Rotate> [--out <dir>] [--dry-run] [--no-recurse] [--allow-vanilla]";

	public IReadOnlySet<string> Options
		=> CommandContext.CommonOptions;

	public IReadOnlySet<string> Flags
		=> CommandContext.CommonFlags;

	public Task<int> RunAsync(CommandContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		if (context.Args.GetString("encoding") == null)
		{
			context.Logger.LogError("--encoding is required");
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
			file => file.DatInfo.ObjectHeader.Encoding == options.Encoding
				? OperationOutcome.Unchanged($"already {options.Encoding}")
				: OperationOutcome.Changed($"{file.DatInfo.ObjectHeader.Encoding} -> {options.Encoding}"),
			options,
			context.Logger);

		return Task.FromResult(CommandContext.Report(result));
	}
}
