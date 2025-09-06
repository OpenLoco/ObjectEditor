using Avalonia.Data.Converters;
using System;

namespace Gui.Converters;

public class NegativeValueConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
	{
		if (value is int intValue)
		{
			return -intValue;
		}
		else if (value is uint uintValue)
		{
			return -uintValue;
		}
		else if (value is short shortValue)
		{
			return -shortValue;
		}
		else if (value is ushort ushortValue)
		{
			return -ushortValue;
		}
		else if (value is sbyte byteValue)
		{
			return -byteValue;
		}
		else if (value is byte sbyteValue)
		{
			return -sbyteValue;
		}

		return Avalonia.Data.BindingNotification.UnsetValue;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
		=> Convert(value, targetType, parameter, culture);
}
