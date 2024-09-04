using Avalonia.Media.Imaging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.IO;

namespace AvaGui.Models
{
	public static class G1ImageConversion
	{
		public static IEnumerable<Bitmap> CreateAvaloniaImages(IEnumerable<Image<Rgba32>> images)
		{
			foreach (var bmp in images)
			{
				yield return CreateAvaloniaImage(bmp);
			}
		}

		public static Bitmap CreateAvaloniaImage(Image<Rgba32> image)
		{
			using (var stream = new MemoryStream())
			{
				image.SaveAsPng(stream);
				stream.Position = 0;
				return new Bitmap(stream);
			}
		}
	}
}
