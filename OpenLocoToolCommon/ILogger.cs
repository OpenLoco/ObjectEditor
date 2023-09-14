namespace OpenLocoToolCommon
{
	public enum LogLevel { Debug2, Debug, Info, Warning, Error };

	public interface ILogger
	{
		void Log(LogLevel level, string message);
		void Debug2(string message) => Log(LogLevel.Debug2, message);
		void Debug(string message) => Log(LogLevel.Debug, message);
		void Info(string message) => Log(LogLevel.Info, message);
		void Warning(string message) => Log(LogLevel.Warning, message);
		void Error(string message) => Log(LogLevel.Error, message);

		void Error(Exception ex) => Log(LogLevel.Error, ex.Message);
	}
}