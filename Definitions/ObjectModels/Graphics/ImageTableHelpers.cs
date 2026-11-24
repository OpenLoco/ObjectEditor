using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Definitions.ObjectModels.Graphics;

public static class ImageTableHelpers
{
	public static readonly GraphicsElement OnePixelTransparent = CreateOnePixelTransparent();

	public static readonly GraphicsElement ErrorImage = CreateErrorGraphicsElement(0);

	static GraphicsElement CreateOnePixelTransparent()
	{
		// A 1x1 palette frame whose single index is 0 = transparent.
		var frame = PaletteMapLoader.Current.CreateIndexedImageFrame(1, 1);
		return GraphicsElement.FromIndexed(GraphicsElementFlags.HasTransparency, frame);
	}

	static GraphicsElement CreateErrorGraphicsElement(int index)
	{
		var frame = PaletteMapLoader.Current.ConvertRgba32ImageToIndexedImage(CreateErrorImage(), GraphicsElementFlags.None);
		return GraphicsElement.FromIndexed(GraphicsElementFlags.None, frame, 0, 0, 0, "<no-image>", index);
	}

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

	public static GraphicsElement GetErrorGraphicsElement(int index)
		=> CreateErrorGraphicsElement(index);
}
