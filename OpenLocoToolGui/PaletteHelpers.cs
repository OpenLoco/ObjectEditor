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

	public static class ImageHelpers
	{
		public static unsafe Color GetPixel(BitmapData d, int X, int Y)
		{
			var ptr = GetPtrToFirstPixel(d, X, Y);
			return Color.FromArgb(ptr[3], ptr[2], ptr[1], ptr[0]);
		}

		public static unsafe void SetPixel(BitmapData d, Point p, Color c)
			=> SetPixel(d, p.X, p.Y, c);

		public static unsafe void SetPixel(BitmapData d, int X, int Y, Color c)
			=> SetPixel(GetPtrToFirstPixel(d, X, Y), c);

		public static unsafe byte* GetPtrToFirstPixel(BitmapData d, int X, int Y)
			=> (byte*)d.Scan0.ToPointer() + (Y * d.Stride) + (X * (Image.GetPixelFormatSize(d.PixelFormat) / 8));

		public static unsafe void SetPixel(byte* ptr, Color c)
		{
			ptr[0] = c.B; // Blue
			ptr[1] = c.G; // Green
			ptr[2] = c.R; // Red
			ptr[3] = c.A; // Alpha
		}
	}
}
