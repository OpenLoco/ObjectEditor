namespace Common
{
	public static class DateOnlyExtensions
	{
		extension(DateOnly)
		{
			public static DateOnly Today
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

	// dummy helper class to get VS to detect the static property above
	// can remove this in VS 2022 17.16
	//public static class Foo
	//{
	//	public static DateOnly Get() => DateOnly.Now;
	//}
}
