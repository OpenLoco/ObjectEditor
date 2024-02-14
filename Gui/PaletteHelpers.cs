using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace OpenLoco.ObjectEditor.Gui
{
	public static class PaletteHelpers
	{
		//public static System.Drawing.Color[] PaletteFromBitmap(Bitmap img)
		//{
		//	var palette = new System.Drawing.Color[256];
		//	var rect = new System.Drawing.Rectangle(0, 0, img.Width, img.Height);
		//	var imgData = img.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
		//	for (var y = 0; y < img.Width; ++y)
		//	{
		//		for (var x = 0; x < img.Height; ++x)
		//		{
		//			palette[(y * img.Height) + x] = ImageHelpers.GetPixel(imgData, x, y);
		//		}
		//	}

		//	img.UnlockBits(imgData);
		//	return palette;
		//}

		public static SixLabors.ImageSharp.Color[] PaletteFromBitmapIS(Image<Rgb24> img)
		{
			var palette = new SixLabors.ImageSharp.Color[256];

			for (var y = 0; y < img.Height; ++y)
			{
				for (var x = 0; x < img.Width; ++x)
				{
					palette[(y * img.Height) + x] = img[x, y];
				}
			}

			return palette;
		}

		//public static byte[] Palettise(Bitmap img)
		//{
		//	var pixels = img.Width * img.Height;
		//	var bytes = new byte[pixels];

		//	var rect = new Rectangle(0, 0, img.Width, img.Height);
		//	var imgData = img.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

		//	for (var y = 0; y < img.Height; ++y)
		//	{
		//		for (var x = 0; x < img.Width; ++x)
		//		{
		//			var paletteIndex = (y * img.Width) + x;
		//			bytes[paletteIndex] = ColorToPaletteIndex(ImageHelpers.GetPixel(imgData, x, y));
		//		}
		//	}

		//	return bytes;
		//}

		//static byte ColorToPaletteIndex(Color c) => 0;
	}
}
