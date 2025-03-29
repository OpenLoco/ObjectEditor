using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace OpenLoco.Gui.Models.Converters
{
	public class IsOfTypeConverter : IValueConverter
	{
		public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> value == null || parameter is not Type targetTypeParameter
				? false
				: (object)targetTypeParameter.IsAssignableFrom(value.GetType());

		public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> throw new NotImplementedException();
	}
}
