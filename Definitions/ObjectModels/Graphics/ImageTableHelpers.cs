using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Definitions.ObjectModels.Graphics;

public static class ImageTableHelpers
{
	public static readonly GraphicsImage OnePixelTransparent = GraphicsImage.FromRgba(GraphicsElementFlags.None, new Image<Rgba32>(1, 1, PaletteMap.Transparent.Color.ToPixel<Rgba32>()));

	public static readonly GraphicsImage ErrorImage = GraphicsImage.FromRgba(GraphicsElementFlags.None, CreateErrorImage());

	private static Image<Rgba32> CreateErrorImage()
	{
		// returns a 32x32 image with a red and white checkerboard pattern
		var img = new Image<Rgba32>(32, 32);
		for (var y = 0; y < img.Height; y++)
		{
			for (var x = 0; x < img.Width; x++)
			{
				var isRed = ((x / 8) + (y / 8)) % 2 == 0;
				img[x, y] = (isRed ? Color.Magenta : Color.White).ToPixel<Rgba32>();
			}
		}

		return img;
	}

	public static GraphicsImage GetErrorGraphicsImage(int index)
		=> GraphicsImage.FromRgba(GraphicsElementFlags.None, CreateErrorImage(), 0, 0, 0, "<no-image>", index);
}
