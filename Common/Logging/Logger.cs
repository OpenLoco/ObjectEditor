using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Common.Logging;

// Simple in-process logger that implements the standard Microsoft.Extensions.Logging.ILogger
// while preserving the historical event/queue surface that the GUI's LogWindow binds to.
public sealed class Logger : ILogger
{
	public ConcurrentQueue<LogLine> Logs { get; } = new();
	public LogLevel MinLevel { get; set; } = LogLevel.Debug;

	public event EventHandler<LogAddedEventArgs>? LogAdded;

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
			message = $"{message} - {exception.Message} - {exception.StackTrace}";
		}

		var caller = string.IsNullOrEmpty(eventId.Name) ? string.Empty : $"[{eventId.Name}]";
		var line = new LogLine(DateTime.Now, logLevel, caller, message);
		Logs.Enqueue(line);
		LogAdded?.Invoke(this, new LogAddedEventArgs(line));
	}
}
