namespace Common.Logging;

public record LogLine(DateTime Time, LogLevel Level, string Caller, string Message)
{
	public override string ToString()
		=> $"[{Time:HH:mm:ss}] [{Level}] [{Caller}] {Message}";
}
