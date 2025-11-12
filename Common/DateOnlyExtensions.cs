namespace Common;

public static class DateOnlyExtensions
{
	extension(DateOnly)
	{
		public static DateOnly UtcToday
			=> DateOnly.FromDateTimeOffset(DateTimeOffset.UtcNow);

		public static DateOnly FromDateTimeOffset(DateTimeOffset dateTimeOffset)
			=> DateOnly.FromDateTime(dateTimeOffset.DateTime);
	}

	extension(DateOnly dateOnly)
	{
		public DateTimeOffset ToDateTimeOffset()
			=> new(dateOnly.Year, dateOnly.Month, dateOnly.Day, 0, 0, 0, TimeSpan.Zero);
	}
}
