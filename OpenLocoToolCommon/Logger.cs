namespace OpenLocoToolCommon
{
	public class LogAddedEventArgs(LogLine log) : EventArgs
	{
		public readonly LogLine Log = log;
	}

	public record LogLine
	{
		public DateTime Time;
		public LogLevel Level;
		public string Message = null!;

		public override string ToString()
			=> $"[{Time}] [{Level}] {Message}";
	}

	public class Logger : ILogger
	{
		public readonly List<LogLine> Logs = [];
		public LogLevel Level = LogLevel.Info;

		public event EventHandler<LogAddedEventArgs> LogAdded;

		public void Log(LogLevel level, string message)
		{
			var log = new LogLine { Time = DateTime.Now, Level = level, Message = message };
			Logs.Add(log);

			if (Level <= level)
			{
				LogAdded?.Invoke(this, new LogAddedEventArgs(log));
			}
		}
	}
}
