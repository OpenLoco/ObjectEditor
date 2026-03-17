using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Common.Logging;

public class Logger : ILogger
{
	public readonly ConcurrentQueue<LogLine> Logs = [];
	public LogLevel Level = LogLevel.Debug;

	public event EventHandler<LogAddedEventArgs>? LogAdded;

	[UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026", Justification = "StackFrame.GetMethod() is used only for diagnostic logging of the caller's class name and is safe to call even if metadata is trimmed.")]
	public void Log(LogLevel level, string message, string callerMemberName = "", string sourceFilePath = "", int sourceLineNumber = 0)
	{
		// Get the class name using reflection
		var frame = new StackFrame(2); // Skip the Log methods
		var method = frame.GetMethod();
		var className = method?.DeclaringType?.Name ?? "<unknown_class>";

		//var fullCaller = $"[{className}.{callerMemberName}] ({sourceFilePath}:{sourceLineNumber})";
		var caller = $"[{className}.{callerMemberName}]";

		var log = new LogLine(DateTime.Now, level, caller, message);
		Logs.Enqueue(log);

		if (Level <= level)
		{
			LogAdded?.Invoke(this, new LogAddedEventArgs(log));
		}
	}
}
