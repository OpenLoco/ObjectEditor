using Definitions.ObjectModels;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Gui.ViewModels.Graphics;

public static class Constants
{
	public static readonly Image<Rgba32> OnePixelTransparent = new(1, 1, PaletteMap.Transparent.Color);

	public static readonly Image<Rgba32> ErrorImage = CreateErrorImage();

	private static Image<Rgba32> CreateErrorImage()
	{
		// returns a 32x32 image with a red and white checkerboard pattern
		var img = new Image<Rgba32>(32, 32);
		for (var y = 0; y < img.Height; y++)
		{
			for (var x = 0; x < img.Width; x++)
			{
				var isRed = ((x / 8) + (y / 8)) % 2 == 0;
				img[x, y] = isRed ? Color.Magenta : Color.White;
			}
		}
		return img;
	}
}
