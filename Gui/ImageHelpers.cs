using System.Drawing.Imaging;

namespace OpenLoco.ObjectEditor.Gui
{
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
