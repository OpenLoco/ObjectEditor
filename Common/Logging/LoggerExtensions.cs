using Microsoft.Extensions.Logging;

namespace Common.Logging;

public static class LoggerExtensions
{
	public static void LogError(this ILogger logger, Exception ex)
		=> logger.LogError(ex, "{Message}", ex.Message);

	public static void LogError(this ILogger logger, Exception ex, string message)
		=> logger.LogError(ex, "{Message}", message);
}
