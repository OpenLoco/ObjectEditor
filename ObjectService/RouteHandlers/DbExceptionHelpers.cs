namespace ObjectService.RouteHandlers;

/// <summary>
/// Helpers for turning database exceptions into HTTP responses.
/// </summary>
public static class DbExceptionHelpers
{
	/// <summary>
	/// Returns <see langword="true"/> when the exception is, or wraps, a unique-constraint violation
	/// (SQLite reports these as "UNIQUE constraint failed: ..."). Used to translate a duplicate name
	/// into a 409 instead of a 500.
	/// </summary>
	public static bool IsUniqueConstraintViolation(Exception exception)
	{
		for (var current = exception; current != null; current = current.InnerException)
		{
			if (current.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}

		return false;
	}
}