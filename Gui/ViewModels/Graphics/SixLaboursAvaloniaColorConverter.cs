using SixLabors.ImageSharp.PixelFormats;
using SLColour = SixLabors.ImageSharp.Color;
using AvaColour = Avalonia.Media.Color;

namespace Gui.ViewModels.Graphics;

public static class SixLaboursAvaloniaColorConverter
{
	public static AvaColour ToAvaloniaColor(this SLColour color)
	{
		var pixel = color.ToPixel<Rgba32>();
		return AvaColour.FromArgb(pixel.A, pixel.R, pixel.G, pixel.B);
	}

	public static SLColour ToSixLaborsColor(this AvaColour color)
		=> SLColour.FromRgba(color.R, color.G, color.B, color.A);
}
