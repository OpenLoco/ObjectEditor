using OpenLoco.Dat.Types;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;

namespace OpenLoco.Dat
{
	public static class PaletteHelpers
	{
		//public static byte[] ConvertRgba32ImageToG1Data(this PaletteMap paletteMap, Image<Rgba32> img)
		//{
		//	var pixels = img.Width * img.Height;
		//	var bytes = new byte[pixels];

		//	for (var y = 0; y < img.Height; ++y)
		//	{
		//		for (var x = 0; x < img.Width; ++x)
		//		{
		//			var dstIndex = (y * img.Width) + x;
		//			bytes[dstIndex] = paletteMap.ColorToPaletteIndex(img[x, y]);
		//		}
		//	}

		//	return bytes;
		//}

		public static byte[] ConvertRgb24ImageToG1Data(this PaletteMap paletteMap, Image<Rgb24> img)
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

		public static Image<Rgb24> ConvertG1IndexedToRgb24Bitmap(this PaletteMap paletteMap, G1Element32 g1Element)
		{
			var image = new Image<Rgb24>(g1Element.Width, g1Element.Height);
			for (var y = 0; y < g1Element.Height; y++)
			{
				for (var x = 0; x < g1Element.Width; x++)
				{
					var index = (y * g1Element.Width) + x;
					var paletteIndex = g1Element.ImageData[index];

					image[x, y] = paletteIndex == 0 && g1Element.Flags.HasFlag(G1ElementFlags.HasTransparency)
						? Color.Magenta.ToPixel<Rgb24>()
						: paletteMap.Palette[paletteIndex].Color.ToPixel<Rgb24>();
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
			var rr = c2.ToPixel<Rgb24>().R - c1.ToPixel<Rgb24>().R;
			var gg = c2.ToPixel<Rgb24>().G - c1.ToPixel<Rgb24>().G;
			var bb = c2.ToPixel<Rgb24>().B - c1.ToPixel<Rgb24>().B;

			return (rr * rr) + (gg * gg) + (bb * bb);
		}
	}
}
