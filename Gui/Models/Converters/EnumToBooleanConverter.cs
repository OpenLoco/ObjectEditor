using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace OpenLoco.Gui.Models.Converters
{
	public class EnumToBooleanConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is Enum enumValue && parameter is Enum enumParameter)
			{
				return enumValue.Equals(enumParameter);
			}
			return false;
		}

		public object? ConvertBack(object? value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value is bool boolValue && boolValue && parameter is Enum enumParameter)
			{
				return enumParameter;
			}
			return null;
		}
	}
}
