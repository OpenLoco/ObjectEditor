using System.Drawing.Imaging;

namespace OpenLocoToolGui
{
	public static class PaletteHelpers
	{
		public static Color[] PaletteFromBitmap(Bitmap img)
		{
			var palette = new Color[256];
			var rect = new Rectangle(0, 0, img.Width, img.Height);
			var imgData = img.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
			for (var y = 0; y < img.Width; ++y)
			{
				for (var x = 0; x < img.Height; ++x)
				{
					palette[(y * img.Height) + x] = ImageHelpers.GetPixel(imgData, x, y);
				}
			}

			img.UnlockBits(imgData);
			return palette;
		}

		public static byte[] Palettise(Bitmap img)
		{
			var pixels = img.Width * img.Height;
			var bytes = new byte[pixels];

			var rect = new Rectangle(0, 0, img.Width, img.Height);
			var imgData = img.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

			for (var y = 0; y < img.Height; ++y)
			{
				for (var x = 0; x < img.Width; ++x)
				{
					var paletteIndex = (y * img.Width) + x;
					bytes[paletteIndex] = ColorToPaletteIndex(ImageHelpers.GetPixel(imgData, x, y));
				}
			}

			return bytes;
		}

		static byte ColorToPaletteIndex(Color c) => 0;
	}
}