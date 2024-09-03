using Avalonia.Media.Imaging;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;

namespace AvaGui.Models
{
	public static class G1ImageConversion
	{
		public static IEnumerable<Bitmap> CreateAvaloniaImages(IEnumerable<SKBitmap> skBitmaps)
		{
			foreach (var bmp in skBitmaps)
			{
				yield return CreateAvaloniaImage(bmp);
			}
		}

		public static Bitmap CreateAvaloniaImage(SKBitmap skBitmap)
		{
			using (var stream = new MemoryStream())
			{
				_ = skBitmap.Encode(stream, SKEncodedImageFormat.Png, 100);
				stream.Position = 0;
				return new Bitmap(stream);
			}
		}
	}
}
