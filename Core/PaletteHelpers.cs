using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;

namespace OpenLoco.ObjectEditor
{
	public static class PaletteHelpers
	{
		public static byte[] ConvertRgb24ImageToG1Data(this PaletteMap paletteMap, Image<Rgb24> img)
		{
			var pixels = img.Width * img.Height;
			var bytes = new byte[pixels];

			for (var y = 0; y < img.Height; ++y)
			{
				for (var x = 0; x < img.Width; ++x)
				{
					var dstIndex = (y * img.Width) + x;
					bytes[dstIndex] = paletteMap.ColorToPaletteIndex(img[x, y]);
				}
			}

			return bytes;
		}

		static byte ColorToPaletteIndex(this PaletteMap paletteMap, Color c)
		{
			var reserved = paletteMap.ReservedColours.Where(cc => cc.Color == c);
			if (reserved.Any())
			{
				return reserved.Single().Index;
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
