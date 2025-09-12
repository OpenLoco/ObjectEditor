using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;
using AvaColour = Avalonia.Media.Color;

namespace Gui.Converters;

public class ColourArrayToGradientStopsConverter : IValueConverter
{
	public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		if (value is AvaColour[] colours)
		{
			var stops = new GradientStops();
			var segmentWidth = 1.0 / colours.Length;

			for (var i = 0; i < colours.Length; i++)
			{
				// Add a stop at the beginning of the segment
				stops.Add(new GradientStop(colours[i], i * segmentWidth));

				// Add a second stop at the end of the segment to create a sharp transition
				stops.Add(new GradientStop(colours[i], (i + 1) * segmentWidth));
			}

			return stops;
		}

		return new GradientStops();
	}

	public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
	{
		throw new NotImplementedException();
	}
}
