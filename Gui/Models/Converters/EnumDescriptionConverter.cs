using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;
using System;
using System.Globalization;

namespace OpenLoco.Gui.Models.Converters
{
	public class EnumDescriptionConverter : MarkupExtension, IValueConverter
	{
		public override object ProvideValue(IServiceProvider serviceProvider)
			=> this;

		public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
		{
			var type = value?.GetType();
			var text = value?.ToString();
			return type?.IsEnum != true || string.IsNullOrEmpty(text)
				? null
				: (object?)$"{type.Name}_{text}";
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
			=> null;
	}
}
