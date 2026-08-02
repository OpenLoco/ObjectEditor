using Definitions.ObjectModels.Graphics;
using Microsoft.Extensions.Logging;
using Shared.Files;

namespace Cli.Commands;

public sealed class ExportImagesCommand : ICommand
{
	public string Name
		=> "export-images";

	public string Summary
		=> "Export an object's images as PNGs plus a sprites.json offsets file";

	public string Usage
		=> "locoobj export-images <file-or-directory> --out <dir> [--use-names] [--palette <png>] [--no-recurse]";

	public IReadOnlySet<string> Options { get; } = new HashSet<string>(CommandContext.CommonOptions, StringComparer.OrdinalIgnoreCase)
	{
		"use-names",
	};

	public IReadOnlySet<string> Flags { get; } = new HashSet<string>(CommandContext.CommonFlags, StringComparer.OrdinalIgnoreCase)
	{
		"use-names",
	};

	public async Task<int> RunAsync(CommandContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		var outputRoot = context.OutputPath;
		if (string.IsNullOrEmpty(outputRoot))
		{
			context.Logger.LogError("--out <dir> is required");
			return ExitCodes.UsageError;
		}

		if (!context.TryResolveInputs(out var files, out var inputRoot))
		{
			return ExitCodes.UsageError;
		}

		var useNames = context.Args.Has("use-names");
		var perObjectFolder = files.Count > 1;
		var failed = 0;

		foreach (var fileName in files)
		{
			try
			{
				var file = ObjectFile.Load(fileName, context.Logger, context.PaletteMap);
				if (file?.LocoObject.ImageTable == null)
				{
					context.Logger.LogWarning("\"{FileName}\" has no image table - skipping", fileName);
					continue;
				}

				var targetDir = perObjectFolder
					? Path.Combine(outputRoot, Path.GetFileNameWithoutExtension(fileName))
					: outputRoot;

				var count = await ImageTableIo.ExportAsync(file.LocoObject.ImageTable, targetDir, useNames, context.Logger);
				Console.WriteLine($"ok   {fileName}: exported {count} image(s) to \"{targetDir}\"");
			}
			catch (Exception ex)
			{
				context.Logger.LogError(ex, "Failed to export images from \"{FileName}\"", fileName);
				Console.WriteLine($"FAIL {fileName}: {ex.Message}");
				failed++;
			}
		}

		return failed == 0 ? ExitCodes.Success : ExitCodes.OperationFailed;
	}
}
