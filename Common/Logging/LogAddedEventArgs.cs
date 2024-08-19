namespace OpenLoco.Common.Logging
{
	public class LogAddedEventArgs(LogLine log) : EventArgs
	{
		public readonly LogLine Log = log;
	}
}
