using OpenLoco.Dat.Types;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace OpenLoco.Dat
{
	public static class PaletteHelpers
	{
		public static byte[] ConvertRgb32ImageToG1Data(this PaletteMap paletteMap, Image<Rgba32> img)
		{
			var pixels = img.Width * img.Height;
			var bytes = new byte[pixels];

			for (var y = 0; y < img.Height; ++y)
			{
				for (var x = 0; x < img.Width; ++x)
				{
					var index = (y * img.Width) + x;
					bytes[index] = paletteMap.ColorToPaletteIndex(img[x, y]);
				}
			}

			return bytes;
		}

		public static Image<Rgba32> ConvertG1ToRgb32Bitmap(this PaletteMap paletteMap, G1Element32 g1Element)
		{
			var image = new Image<Rgba32>(g1Element.Width, g1Element.Height);

			for (var y = 0; y < g1Element.Height; y++)
			{
				for (var x = 0; x < g1Element.Width; x++)
				{
					var index = (y * g1Element.Width) + x;
					var paletteIndex = g1Element.ImageData[index];

					image[x, y] = paletteIndex == 0 && g1Element.Flags.HasFlag(G1ElementFlags.HasTransparency)
						? Color.Transparent
						: paletteMap.Palette[paletteIndex].Color;
				}
			}

			return image;
		}

		static byte ColorToPaletteIndex(this PaletteMap paletteMap, Color c)
		{
			var reserved = paletteMap.ReservedColours.Where(cc => cc.Color == c);
			if (reserved.Any())
			{
				return reserved.First().Index;
			}

			return paletteMap.ValidColours.MinBy(vc => DistanceSquared(c, vc.Color)).Index;
		}

		static int DistanceSquared(Color c1, Color c2)
		{
			var p1 = c1.ToPixel<Rgba32>();
			var p2 = c2.ToPixel<Rgba32>();

			var rr = p2.R - p1.R;
			var gg = p2.G - p1.G;
			var bb = p2.B - p1.B;

			return (rr * rr) + (gg * gg) + (bb * bb);
		}
	}
}
