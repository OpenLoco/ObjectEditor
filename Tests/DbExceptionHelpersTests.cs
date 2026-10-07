using NUnit.Framework;
using ObjectService.RouteHandlers;

namespace Tests;

[TestFixture]
public class DbExceptionHelpersTests
{
	[Test]
	public void IsUniqueConstraintViolation_DetectsNestedSqliteMessage()
	{
		var inner = new InvalidOperationException("SQLite Error 19: 'UNIQUE constraint failed: ObjectPacks.Name'.");
		var exception = new Exception("outer", inner);

		Assert.That(DbExceptionHelpers.IsUniqueConstraintViolation(exception), Is.True);
	}

	[Test]
	public void IsUniqueConstraintViolation_IsFalseForOtherErrors()
	{
		Assert.That(DbExceptionHelpers.IsUniqueConstraintViolation(new InvalidOperationException("boom")), Is.False);
	}

	[Test]
	public void IsUniqueConstraintViolation_IsFalseForForeignKeyViolation()
	{
		var exception = new Exception("outer", new InvalidOperationException("SQLite Error 19: 'FOREIGN KEY constraint failed'."));

		Assert.That(DbExceptionHelpers.IsUniqueConstraintViolation(exception), Is.False);
	}
}