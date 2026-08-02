using Dat.Data;
using Definitions.ObjectModels.Graphics;
using Microsoft.Extensions.Logging;
using Shared.Files;
using Shared.Operations;

namespace Cli;

public sealed class CommandContext(CommandLine commandLine, ILogger logger)
{
	public CommandLine Args { get; } = commandLine;

	public ILogger Logger { get; } = logger;

	public PaletteMap PaletteMap
		=> field ??= PaletteMapLoader.Load(Args.GetString("palette"));

	public static IReadOnlySet<string> CommonFlags { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"dry-run", "no-recurse", "allow-vanilla", "verbose", "quiet", "help",
	};

	public static IReadOnlySet<string> CommonOptions { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
	{
		"dry-run", "no-recurse", "allow-vanilla", "verbose", "quiet", "help", "out", "encoding", "palette",
	};

	public bool DryRun
		=> Args.Has("dry-run");

	public bool Recursive
		=> !Args.Has("no-recurse");

	public bool AllowSavingAsVanillaObject
		=> Args.Has("allow-vanilla");

	public string? OutputPath
		=> Args.GetString("out");

	public bool TryGetEncoding(out SawyerEncoding? encoding)
	{
		encoding = null;
		var raw = Args.GetString("encoding");

		if (string.IsNullOrEmpty(raw))
		{
			return true;
		}

		if (!Enum.TryParse<SawyerEncoding>(raw, ignoreCase: true, out var parsed))
		{
			Logger.LogError("Unknown encoding \"{Encoding}\". Valid values: {Valid}", raw, string.Join(", ", Enum.GetNames<SawyerEncoding>()));
			return false;
		}

		encoding = parsed;
		return true;
	}

	public bool TryResolveInputs(out IReadOnlyList<string> files, out string inputRoot)
	{
		files = [];
		inputRoot = string.Empty;

		var path = Args.Positionals.Count > 0 ? Args.Positionals[0] : null;

		if (string.IsNullOrEmpty(path))
		{
			Logger.LogError("No input path was given");
			return false;
		}

		files = ObjectFile.EnumerateDatFiles(path, Recursive);

		if (files.Count == 0)
		{
			Logger.LogError("No .dat files found at \"{Path}\"", path);
			return false;
		}

		inputRoot = Directory.Exists(path) ? path : Path.GetDirectoryName(path) ?? string.Empty;
		return true;
	}

	public bool TryBuildBatchOptions(string inputRoot, out BatchOptions options, bool withPalette = false)
	{
		options = new BatchOptions();

		if (!TryGetEncoding(out var encoding))
		{
			return false;
		}

		options = new BatchOptions
		{
			OutputDirectory = OutputPath,
			InputRoot = inputRoot,
			Encoding = encoding,
			AllowSavingAsVanillaObject = AllowSavingAsVanillaObject,
			DryRun = DryRun,
			PaletteMap = withPalette ? PaletteMap : null,
		};

		return true;
	}

	public static int Report(BatchResult result)
	{
		ArgumentNullException.ThrowIfNull(result);

		foreach (var item in result.Items)
		{
			Console.WriteLine($"{(item.Succeeded ? "ok  " : "FAIL")} {item.FileName}: {item.Message}");
		}

		Console.WriteLine($"{result.SucceededCount} succeeded, {result.FailedCount} failed");
		return result.FailedCount == 0 ? ExitCodes.Success : ExitCodes.OperationFailed;
	}

	public bool ValidateOptions(IReadOnlySet<string> known)
	{
		var unknown = Args.UnknownOptions(known).ToList();
		if (unknown.Count == 0)
		{
			return true;
		}

		Logger.LogError("Unknown option(s): {Unknown}", string.Join(", ", unknown.Select(x => $"--{x}")));
		return false;
	}
}
