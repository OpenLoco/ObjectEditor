using Microsoft.Extensions.Logging;

namespace Cli;

public sealed class ConsoleLogger(LogLevel minLevel) : ILogger
{
	public LogLevel MinLevel { get; set; } = minLevel;

	public IDisposable? BeginScope<TState>(TState state) where TState : notnull
		=> null;

	public bool IsEnabled(LogLevel logLevel)
		=> logLevel != LogLevel.None && logLevel >= MinLevel;

	public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
	{
		ArgumentNullException.ThrowIfNull(formatter);

		if (!IsEnabled(logLevel))
		{
			return;
		}

		var message = formatter(state, exception);
		if (exception != null)
		{
			message = $"{message} - {exception.Message}";
		}

		Console.Error.WriteLine($"{Prefix(logLevel)} {message}");
	}

	static string Prefix(LogLevel level)
		=> level switch
		{
			LogLevel.Trace => "trce:",
			LogLevel.Debug => "dbug:",
			LogLevel.Information => "info:",
			LogLevel.Warning => "warn:",
			LogLevel.Error => "fail:",
			LogLevel.Critical => "crit:",
			_ => "     ",
		};
}
