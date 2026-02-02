using Avalonia.Media.Imaging;
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
}
