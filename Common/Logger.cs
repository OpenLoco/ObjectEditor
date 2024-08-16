using System.Collections.Concurrent;

namespace OpenLoco.Dat.Logging
{
	public class LogAddedEventArgs(LogLine log) : EventArgs
	{
		public readonly LogLine Log = log;
	}

	public record LogLine(DateTime Time, LogLevel Level, string Caller, string Message)
	{
		public override string ToString()
			=> $"[{Time}] [{Level}] [{Caller}] {Message}";
	}

	public class Logger : ILogger
	{
		public readonly ConcurrentQueue<LogLine> Logs = [];
		public LogLevel Level = LogLevel.Info;

		public event EventHandler<LogAddedEventArgs>? LogAdded;

		public void Log(LogLevel level, string message, string callerMemberName = "")
		{
			var log = new LogLine(DateTime.Now, level, callerMemberName, message);
			Logs.Enqueue(log);

			if (Level <= level)
			{
				LogAdded?.Invoke(this, new LogAddedEventArgs(log));
			}
		}
	}
}
