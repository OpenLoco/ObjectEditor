using OpenLoco.Dat.Types;
using SkiaSharp;

namespace OpenLoco.Dat
{
	public static class PaletteHelpers
	{
		public static byte[] ConvertRgb32ImageToG1Data(this PaletteMap paletteMap, SKBitmap img)
		{
			var pixels = img.Width * img.Height;
			var bytes = new byte[pixels];

			for (var y = 0; y < img.Height; ++y)
			{
				for (var x = 0; x < img.Width; ++x)
				{
					var index = (y * img.Width) + x;
					bytes[index] = paletteMap.ColorToPaletteIndex(img.GetPixel(x, y));
				}
			}

			return bytes;
		}

		public static SKBitmap ConvertG1IndexedToRgb32Bitmap(this PaletteMap paletteMap, G1Element32 g1Element)
		{
			var image = new SKBitmap(g1Element.Width, g1Element.Height, SKColorType.Rgba8888, SKAlphaType.Premul);

			// Get the pixel array pointer
			var pixelsPtr = image.GetPixels();

			unsafe
			{
				var pixels = (byte*)pixelsPtr.ToPointer();
				for (var y = 0; y < g1Element.Height; y++)
				{
					for (var x = 0; x < g1Element.Width; x++)
					{
						var index = (y * g1Element.Width) + x;
						var paletteIndex = g1Element.ImageData[index];

						var col = paletteIndex == 0 && g1Element.Flags.HasFlag(G1ElementFlags.HasTransparency)
							? SKColors.Transparent
							: paletteMap.Palette[paletteIndex].Color;

						// Calculate the offset into the pixel array
						var pixelOffset = (y * image.RowBytes) + (x * 4);

						// Set the RGB values directly into the pixel array
						pixels[pixelOffset] = col.Red;
						pixels[pixelOffset + 1] = col.Green;
						pixels[pixelOffset + 2] = col.Blue;
						pixels[pixelOffset + 3] = col.Alpha;
					}
				}
			}

			return image;
		}

		static byte ColorToPaletteIndex(this PaletteMap paletteMap, SKColor c)
		{
			var reserved = paletteMap.ReservedColours.Where(cc => cc.Color == c);
			if (reserved.Any())
			{
				return reserved.First().Index;
			}

			return paletteMap.ValidColours.MinBy(vc => DistanceSquared(c, vc.Color)).Index;
		}

		static int DistanceSquared(SKColor c1, SKColor c2)
		{
			var rr = c2.Red - c1.Red;
			var gg = c2.Green - c1.Green;
			var bb = c2.Blue - c1.Blue;

			return (rr * rr) + (gg * gg) + (bb * bb);
		}
	}
}
