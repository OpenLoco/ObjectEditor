namespace Common;

public static class EnumExtensions
{
	public static T ToggleFlag<T>(this T enumValue, T flag, bool enable) where T : Enum
	{
		// Get the underlying type of the enum
		var underlyingType = Enum.GetUnderlyingType(typeof(T));

		// Convert the enum values to long to handle all integer types safely
		var value = Convert.ToInt64(enumValue);
		var flagValue = Convert.ToInt64(flag);

		if (enable)
		{
			// Set the flag using bitwise OR
			value |= flagValue;
		}
		else
		{
			// Unset the flag using bitwise AND with the bitwise NOT of the flag
			value &= ~flagValue;
		}

		// Convert the modified long back to the original underlying type
		var result = Convert.ChangeType(value, underlyingType);

		// Cast the result to the generic type T and return
		return (T)result;
	}
}
