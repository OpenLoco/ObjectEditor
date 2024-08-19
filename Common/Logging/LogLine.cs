namespace OpenLoco.Common.Logging
{
	public record LogLine(DateTime Time, LogLevel Level, string Caller, string Message)
	{
		public override string ToString()
			=> $"[{Time}] [{Level}] [{Caller}] {Message}";
	}
}
