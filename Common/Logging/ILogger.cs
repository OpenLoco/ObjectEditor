using System.Runtime.CompilerServices;

namespace Common.Logging;

public interface ILogger
{
	public event EventHandler<LogAddedEventArgs>? LogAdded;

	void Log(LogLevel level, string message, string callerMemberName, string sourceFilePath, int sourceLineNumber);

	void Debug2(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		=> Log(LogLevel.Debug2, message, callerMemberName, sourceFilePath, sourceLineNumber);

	void Debug(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		=> Log(LogLevel.Debug, message, callerMemberName, sourceFilePath, sourceLineNumber);

	void Info(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		=> Log(LogLevel.Info, message, callerMemberName, sourceFilePath, sourceLineNumber);

	void Warning(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		=> Log(LogLevel.Warning, message, callerMemberName, sourceFilePath, sourceLineNumber);

	void Error(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		=> Log(LogLevel.Error, message, callerMemberName, sourceFilePath, sourceLineNumber);

	void Error(Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		=> Log(LogLevel.Error, $"{ex.Message} - {ex.StackTrace}", callerMemberName, sourceFilePath, sourceLineNumber);

	void Error(string message, Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		=> Log(LogLevel.Error, $"{message} - {ex.Message} - {ex.StackTrace}", callerMemberName, sourceFilePath, sourceLineNumber);
}
