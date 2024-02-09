using System.Runtime.CompilerServices;

namespace OpenLoco.ObjectEditor.Logging
{
	public enum LogLevel { Debug2, Debug, Info, Warning, Error };

	public interface ILogger
	{
		// should be 'private'
		void Log(LogLevel level, string message, string callerMemberName);

		void Debug2(string message, [CallerMemberName] string callerMemberName = "")
			=> Log(LogLevel.Debug2, message, callerMemberName);

		void Debug(string message, [CallerMemberName] string callerMemberName = "")
			=> Log(LogLevel.Debug, message, callerMemberName);

		void Info(string message, [CallerMemberName] string callerMemberName = "")
			=> Log(LogLevel.Info, message, callerMemberName);

		void Warning(string message, [CallerMemberName] string callerMemberName = "")
			=> Log(LogLevel.Warning, message, callerMemberName);

		void Error(string message, [CallerMemberName] string callerMemberName = "")
			=> Log(LogLevel.Error, message, callerMemberName);

		void Error(Exception ex, [CallerMemberName] string callerMemberName = "")
			=> Log(LogLevel.Error, $"{ex.Message} - {ex.StackTrace}", callerMemberName);

		void Error(string message, Exception ex, [CallerMemberName] string callerMemberName = "")
			=> Log(LogLevel.Error, $"{message} - {ex.Message} - {ex.StackTrace}", callerMemberName);
	}
}