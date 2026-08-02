using Dat.Data;
using Definitions.ObjectModels.Graphics;
using Microsoft.Extensions.Logging;
using Shared.Files;

namespace Shared.Operations;

public sealed record BatchItemResult(string FileName, bool Succeeded, string Message);

public sealed record BatchResult(IReadOnlyList<BatchItemResult> Items)
{
	public int SucceededCount
		=> Items.Count(x => x.Succeeded);

	public int FailedCount
		=> Items.Count(x => !x.Succeeded);
}

public sealed record OperationOutcome(bool Modified, string Message)
{
	public static OperationOutcome Unchanged(string message)
		=> new(false, message);

	public static OperationOutcome Changed(string message)
		=> new(true, message);
}

public sealed record BatchOptions
{
	public string? OutputDirectory { get; init; }

	public string? InputRoot { get; init; }

	public SawyerEncoding? Encoding { get; init; }

	public bool AllowSavingAsVanillaObject { get; init; }

	public bool DryRun { get; init; }

	public PaletteMap? PaletteMap { get; init; }
}

public static class BatchProcessor
{
	public static BatchResult Run(IEnumerable<string> fileNames, Func<LocoObjectFile, OperationOutcome> operation, BatchOptions options, ILogger logger)
	{
		ArgumentNullException.ThrowIfNull(fileNames);
		ArgumentNullException.ThrowIfNull(operation);
		ArgumentNullException.ThrowIfNull(options);
		ArgumentNullException.ThrowIfNull(logger);

		var results = new List<BatchItemResult>();

		foreach (var fileName in fileNames)
		{
			results.Add(RunOne(fileName, operation, options, logger));
		}

		return new BatchResult(results);
	}

	static BatchItemResult RunOne(string fileName, Func<LocoObjectFile, OperationOutcome> operation, BatchOptions options, ILogger logger)
	{
		try
		{
			var file = ObjectFile.Load(fileName, logger, options.PaletteMap);
			if (file == null)
			{
				return new BatchItemResult(fileName, false, "failed to load");
			}

			var outcome = operation(file);

			if (!outcome.Modified)
			{
				return new BatchItemResult(fileName, true, outcome.Message);
			}

			var outputFileName = ResolveOutputFileName(fileName, options);

			if (options.DryRun)
			{
				return new BatchItemResult(fileName, true, $"{outcome.Message} (dry run, would write \"{outputFileName}\")");
			}

			var outputDir = Path.GetDirectoryName(outputFileName);
			if (!string.IsNullOrEmpty(outputDir))
			{
				_ = Directory.CreateDirectory(outputDir);
			}

			return ObjectFile.SaveDat(file, outputFileName, logger, options.Encoding, allowSavingAsVanillaObject: options.AllowSavingAsVanillaObject)
				? new BatchItemResult(fileName, true, outcome.Message)
				: new BatchItemResult(fileName, false, "failed to save");
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Unhandled error processing \"{FileName}\"", fileName);
			return new BatchItemResult(fileName, false, ex.Message);
		}
	}

	public static string ResolveOutputFileName(string inputFileName, BatchOptions options)
	{
		ArgumentNullException.ThrowIfNull(options);

		if (string.IsNullOrEmpty(options.OutputDirectory))
		{
			return inputFileName;
		}

		var relative = string.IsNullOrEmpty(options.InputRoot)
			? Path.GetFileName(inputFileName)
			: Path.GetRelativePath(options.InputRoot, inputFileName);

		return Path.Combine(options.OutputDirectory, relative);
	}
}
