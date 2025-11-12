using NUnit.Framework;

namespace Common.Tests;

[TestFixture]
public class DateOnlyTests
{
	[Test]
	public void Today()
	{
		var today = DateTimeOffset.UtcNow;
		Assert.That(DateOnly.UtcToday, Is.EqualTo(new DateOnly(today.Year, today.Month, today.Day)));
	}
}
