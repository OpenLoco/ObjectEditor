namespace Common
{
	public static class DateOnlyExtensions
	{
		extension(DateOnly)
		{
			public static DateOnly Now
				=> DateOnly.Now;

			public static DateOnly FromDateTimeOffset(DateTimeOffset dateTimeOffset)
				=> DateOnly.FromDateTime(dateTimeOffset.DateTime);
		}
	}

	// dummy helper class to get VS to detect the static property above
	// can remove this in VS 2022 17.16
	public static class Foo
	{
		public static DateOnly Get() => DateOnly.Now;
	}
}
