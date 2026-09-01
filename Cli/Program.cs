using Cli;
using Cli.Commands;
using Definitions.ObjectModels.Graphics;
using Microsoft.Extensions.Logging;

ICommand[] commands =
[
	new StripImagesCommand(),
	new ExportImagesCommand(),
	new ImportImagesCommand(),
	new CropCommand(),
	new OffsetsCommand(),
	new ReencodeCommand(),
	new ValidateCommand(),
	new InfoCommand(),
];

if (args.Length == 0 || args[0] is "-h" or "--help" or "help")
{
	PrintHelp(commands);
	return ExitCodes.Success;
}

var command = commands.FirstOrDefault(x => string.Equals(x.Name, args[0], StringComparison.OrdinalIgnoreCase));

if (command == null)
{
	Console.Error.WriteLine($"Unknown command \"{args[0]}\"");
	PrintHelp(commands);
	return ExitCodes.UsageError;
}

var commandArgs = args[1..];
var flags = new HashSet<string>(command.Flags, StringComparer.OrdinalIgnoreCase);
var commandLine = CommandLine.Parse(commandArgs, flags);

if (commandLine.Has("help"))
{
	Console.WriteLine(command.Summary);
	Console.WriteLine();
	Console.WriteLine(command.Usage);
	return ExitCodes.Success;
}

var minLevel = commandLine.Has("verbose")
	? LogLevel.Debug
	: commandLine.Has("quiet") ? LogLevel.Error : LogLevel.Information;

var logger = new ConsoleLogger(minLevel);
var context = new CommandContext(commandLine, logger);

if (!context.ValidateOptions(command.Options))
{
	Console.Error.WriteLine(command.Usage);
	return ExitCodes.UsageError;
}

var groupConfigs = await ImageTableGroupLoader.LoadDefaultAsync(logger);
if (groupConfigs != null)
{
	ImageTableGrouper.GroupConfigurations = groupConfigs;
}

try
{
	return await command.RunAsync(context);
}
catch (Exception ex) when (ex is not (OutOfMemoryException or StackOverflowException or ThreadAbortException))
{
	logger.LogError(ex, "Unhandled error running \"{Command}\"", command.Name);
	return ExitCodes.OperationFailed;
}

static void PrintHelp(IEnumerable<ICommand> commands)
{
	Console.WriteLine("locoobj - headless OpenLoco object tools");
	Console.WriteLine();
	Console.WriteLine("Usage: locoobj <command> [arguments]");
	Console.WriteLine();
	Console.WriteLine("Commands:");

	foreach (var command in commands)
	{
		Console.WriteLine($"  {command.Name,-14} {command.Summary}");
	}

	Console.WriteLine();
	Console.WriteLine("Common options:");
	Console.WriteLine("  --out <path>       write results here instead of overwriting the input");
	Console.WriteLine("  --encoding <enc>   Uncompressed | RunLengthSingle | RunLengthMulti | Rotate");
	Console.WriteLine("  --palette <png>    use a custom 16x16 palette instead of the built-in one");
	Console.WriteLine("  --dry-run          report what would change without writing anything");
	Console.WriteLine("  --no-recurse       do not descend into subdirectories");
	Console.WriteLine("  --allow-vanilla    permit writing objects with a vanilla object source");
	Console.WriteLine("  --verbose/--quiet  raise or lower log verbosity");
	Console.WriteLine();
	Console.WriteLine("Run 'locoobj <command> --help' for command-specific usage.");
}
