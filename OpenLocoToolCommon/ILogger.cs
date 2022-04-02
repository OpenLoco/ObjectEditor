namespace OpenLocoToolCommon
{
	public enum LogLevel { Debug2, Debug, Info, Warning, Error };

	public interface ILogger
	{
		void Log(LogLevel level, string message);
	}
}