using System.Collections.Concurrent;

namespace OpenLoco.Common.Logging
{
	public class Logger : ILogger
	{
		public readonly ConcurrentQueue<LogLine> Logs = [];
		public LogLevel Level = LogLevel.Debug;

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
