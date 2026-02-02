using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Common.Logging;
using System;
using System.Globalization;

namespace Gui.Converters;

public class LogLevelToRowBrushConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		var type = parameter as string;
		if (string.IsNullOrEmpty(type))
		{
			// Fallback if no parameter is provided
			return Brushes.Black;
		}

		var resourceKey = string.Empty;
		if (type.Equals("Foreground", StringComparison.OrdinalIgnoreCase))
		{
			resourceKey = "Foreground";
		}
		else if (type.Equals("Background", StringComparison.OrdinalIgnoreCase))
		{
			resourceKey = "Background";
		}
		else
		{
			return Brushes.Magenta;
		}

		if (value is LogLine { Level: LogLevel.Error })
		{
			if (Application.Current.TryGetResource($"Danger{resourceKey}Brush", Application.Current.ActualThemeVariant, out var resource) && resource is ISolidColorBrush brush)
			{
				return brush;
			}

			return Brushes.Red;
		}
		else if (value is LogLine { Level: LogLevel.Warning })
		{
			if (Application.Current.TryGetResource($"Warning{resourceKey}Brush", Application.Current.ActualThemeVariant, out var resource) && resource is ISolidColorBrush brush)
			{
				return brush;
			}

			return Brushes.Yellow;
		}
		else
		{
			if (Application.Current.TryGetResource($"Text{resourceKey}Brush", Application.Current.ActualThemeVariant, out var resource) && resource is ISolidColorBrush brush)
			{
				return brush;
			}

			return Brushes.Black;
		}
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		=> null;
}
