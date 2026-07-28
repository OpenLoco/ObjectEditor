namespace Cli;

public sealed class CommandLine
{
	readonly List<string> positionals = [];
	readonly Dictionary<string, string?> options = new(StringComparer.OrdinalIgnoreCase);

	public IReadOnlyList<string> Positionals
		=> positionals;

	public IReadOnlyCollection<string> OptionNames
		=> options.Keys;

	public static CommandLine Parse(IReadOnlyList<string> args, IReadOnlySet<string> flagNames)
	{
		ArgumentNullException.ThrowIfNull(args);
		ArgumentNullException.ThrowIfNull(flagNames);

		var result = new CommandLine();

		for (var i = 0; i < args.Count; i++)
		{
			var arg = args[i];

			if (!arg.StartsWith("--", StringComparison.Ordinal))
			{
				result.positionals.Add(arg);
				continue;
			}

			var body = arg[2..];
			var equals = body.IndexOf('=', StringComparison.Ordinal);

			if (equals >= 0)
			{
				result.options[body[..equals]] = body[(equals + 1)..];
				continue;
			}

			if (flagNames.Contains(body) || i + 1 >= args.Count || args[i + 1].StartsWith("--", StringComparison.Ordinal))
			{
				result.options[body] = null;
				continue;
			}

			result.options[body] = args[++i];
		}

		return result;
	}

	public bool Has(string name)
		=> options.ContainsKey(name);

	public string? GetString(string name, string? defaultValue = null)
		=> options.TryGetValue(name, out var value) && value != null ? value : defaultValue;

	public bool TryGetInt(string name, out int value)
	{
		value = 0;
		return options.TryGetValue(name, out var raw) && int.TryParse(raw, out value);
	}

	public IEnumerable<string> UnknownOptions(IReadOnlySet<string> known)
		=> options.Keys.Where(x => !known.Contains(x));
}
