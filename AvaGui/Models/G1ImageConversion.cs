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

		//static Bitmap G1RGBToRgba32Bitmap(G1Element32 g1Element, int zoom = 1)
		//{
		//	var imageData = g1Element.ImageData;
		//	var writeableBitmap = new WriteableBitmap(
		//		new PixelSize(g1Element.Width * zoom, g1Element.Height * zoom),
		//		new Vector(96, 96),  // DPI
		//		PixelFormat.Rgba8888); // Or a suitable pixel format

		//	using (var lockedBitmap = writeableBitmap.Lock())
		//	{
		//		unsafe
		//		{
		//			var pointer = (uint*)lockedBitmap.Address; // Access pixel data directly

		//			for (var y = 0; y < g1Element.Height; y++)
		//			{
		//				for (var x = 0; x < g1Element.Width; x++)
		//				{
		//					// Calculate pixel index
		//					var index = x + (y * g1Element.Width);

		//					// Set pixel color (example: red)
		//					pointer[index] = 0xFFFF0000;
		//				}
		//			}
		//		}
		//	}

		//	return writeableBitmap;
		//}

		//static Bitmap G1RGBToRgb24Bitmap(G1Element32 g1Element, int zoom = 1)
		//{
		//	var imageData = g1Element.ImageData;
		//	var writeableBitmap = new WriteableBitmap(
		//		new PixelSize(g1Element.Width * zoom, g1Element.Height * zoom),
		//		new Vector(96, 96),  // DPI
		//		PixelFormats.Rgb24); // Or a suitable pixel format

		//	using (var lockedBitmap = writeableBitmap.Lock())
		//	{
		//		unsafe
		//		{
		//			var pointer = (uint*)lockedBitmap.Address; // Access pixel data directly

		//			for (var y = 0; y < g1Element.Height; y++)
		//			{
		//				for (var x = 0; x < g1Element.Width; x++)
		//				{
		//					// Calculate pixel index
		//					var index = x + (y * g1Element.Width);

		//					// Set pixel color (example: red)
		//					pointer[index] = 0xFFFF0000;
		//				}
		//			}
		//		}
		//	}

		//	return writeableBitmap;
		//}

		//static Bitmap G1IndexedToRgba32Bitmap(G1Element32 g1Element, PaletteMap paletteMap, bool useTransparency = false, int zoom = 1)
		//{
		//	var writeableBitmap = new WriteableBitmap(
		//		new PixelSize(g1Element.Width, g1Element.Height),
		//		new Vector(96, 96),  // DPI
		//		PixelFormat.Rgba8888); // Or a suitable pixel format

		//	using (var lockedBitmap = writeableBitmap.Lock())
		//	{
		//		unsafe
		//		{
		//			var ptr = (byte*)lockedBitmap.Address; // Access pixel data directly

		//			for (var y = 0; y < g1Element.Height; y++)
		//			{
		//				for (var x = 0; x < g1Element.Width; x++)
		//				{
		//					var index = x + (y * g1Element.Width);
		//					var paletteIndex = g1Element.ImageData[index];

		//					if (paletteIndex == 0 && useTransparency)
		//					{
		//						ptr += 4;
		//					}
		//					else
		//					{
		//						var colour = paletteMap.Palette[paletteIndex].Color;
		//						var pixel = colour.ToPixel<Rgb24>();

		//						//var ptr = (byte*)pointer;
		//						*ptr++ = pixel.R;
		//						*ptr++ = pixel.G;
		//						*ptr++ = pixel.B;
		//						*ptr++ = 255;
		//					}
		//				}
		//			}
		//		}
		//	}

		//	// bug in avalonia/skiasharp/skia: https://github.com/AvaloniaUI/Avalonia/issues/8444
		//	//return writeableBitmap.CreateScaledBitmap(new PixelSize(g1Element.Width * zoom, g1Element.Height * zoom), BitmapInterpolationMode.None);

		//	return writeableBitmap;
		//}

		//static Bitmap G1IndexedToRgb24Bitmap(G1Element32 g1Element, PaletteMap paletteMap, bool useTransparency = false, int zoom = 1)
		//{
		//	var writeableBitmap = new WriteableBitmap(
		//		new PixelSize(g1Element.Width, g1Element.Height),
		//		new Vector(96, 96),  // DPI
		//		PixelFormats.Rgb24); // Or a suitable pixel format

		//	using (var lockedBitmap = writeableBitmap.Lock())
		//	{
		//		unsafe
		//		{
		//			var ptr = (byte*)lockedBitmap.Address; // Access pixel data directly

		//			for (var y = 0; y < g1Element.Height; y++)
		//			{
		//				for (var x = 0; x < g1Element.Width; x++)
		//				{
		//					var index = x + (y * g1Element.Width);
		//					var paletteIndex = g1Element.ImageData[index];

		//					if (paletteIndex == 0 && useTransparency)
		//					{
		//						//ptr += 3;
		//						*ptr++ = 0xFF;
		//						*ptr++ = 0x00;
		//						*ptr++ = 0xFF;
		//					}
		//					else
		//					{
		//						var colour = paletteMap.Palette[paletteIndex].Color;
		//						var pixel = colour.ToPixel<Rgb24>();

		//						//var ptr = (byte*)pointer;
		//						*ptr++ = pixel.R;
		//						*ptr++ = pixel.G;
		//						*ptr++ = pixel.B;
		//					}
		//				}
		//			}
		//		}
		//	}

		//	// bug in avalonia/skiasharp/skia: https://github.com/AvaloniaUI/Avalonia/issues/8444
		//	//return writeableBitmap.CreateScaledBitmap(new PixelSize(g1Element.Width * zoom, g1Element.Height * zoom), BitmapInterpolationMode.None);

		//	return writeableBitmap;
		//}
	}
}
