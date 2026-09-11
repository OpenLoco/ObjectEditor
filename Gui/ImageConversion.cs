using Avalonia.Media.Imaging;
using Definitions.ObjectModels.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace Gui;

public static class ImageConversion
{
	public static Bitmap ToAvaloniaBitmap(this Image<Rgba32> image)
	{
		using (var stream = new MemoryStream())
		{
			image.SaveAsPng(stream);
			stream.Position = 0;
			return new Bitmap(stream);
		}
	}

	/// <summary>Renders a <see cref="GraphicsElement"/> (decoding it if needed) to an Avalonia <see cref="Bitmap"/> for display.</summary>
	public static Bitmap ToAvaloniaBitmap(this GraphicsElement image, PaletteMap paletteMap)
	{
		using var rgba = image.ToRgba(paletteMap);
		return rgba.ToAvaloniaBitmap();
	}
}
