using Core.Objects;
using Core.Validation;

namespace Cli.Commands;

public sealed class ValidateCommand : ICommand
{
	public string Name
		=> "validate";

	public string Summary
		=> "Validate objects, optionally against the OpenGraphics ruleset";

	public string Usage
		=> "locoobj validate <file-or-directory> [--og] [--no-recurse]";

	public IReadOnlySet<string> Options { get; } = new HashSet<string>(CommandContext.CommonOptions, StringComparer.OrdinalIgnoreCase)
	{
		"og",
	};

	public IReadOnlySet<string> Flags { get; } = new HashSet<string>(CommandContext.CommonFlags, StringComparer.OrdinalIgnoreCase)
	{
		"og",
	};

	public Task<int> RunAsync(CommandContext context)
	{
		ArgumentNullException.ThrowIfNull(context);

		if (!context.TryResolveInputs(out var files, out _))
		{
			return Task.FromResult(ExitCodes.UsageError);
		}

		var includeOg = context.Args.Has("og");
		var failed = 0;

		foreach (var fileName in files)
		{
			var file = ObjectFile.Load(fileName, context.Logger);
			if (file == null)
			{
				Console.WriteLine($"FAIL {fileName}: failed to load");
				failed++;
				continue;
			}

			var errors = ObjectValidation.Validate(file);

			if (includeOg)
			{
				errors.AddRange(ObjectValidation.ValidateForOG(file, context.Logger));
			}

			if (errors.Count == 0)
			{
				Console.WriteLine($"ok   {fileName}");
				continue;
			}

			failed++;
			Console.WriteLine($"FAIL {fileName}: {errors.Count} issue(s)");
			foreach (var error in errors)
			{
				Console.WriteLine($"       {error}");
			}
		}

		Console.WriteLine($"{files.Count - failed} passed, {failed} failed");
		return Task.FromResult(failed == 0 ? ExitCodes.Success : ExitCodes.ValidationFailed);
	}
}
