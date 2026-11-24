namespace Cli;

public interface ICommand
{
	string Name { get; }

	string Summary { get; }

	string Usage { get; }

	IReadOnlySet<string> Options { get; }

	IReadOnlySet<string> Flags { get; }

	Task<int> RunAsync(CommandContext context);
}
