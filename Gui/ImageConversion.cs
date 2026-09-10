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

	/// <summary>Renders a <see cref="GraphicsImage"/> (decoding it if needed) to an Avalonia <see cref="Bitmap"/> for display.</summary>
	public static Bitmap ToAvaloniaBitmap(this GraphicsImage image, PaletteMap paletteMap)
		=> image.ToRgba(paletteMap).ToAvaloniaBitmap();
}
