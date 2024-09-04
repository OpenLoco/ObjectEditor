using OpenLoco.Dat.Types;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using Zenith.Core;

namespace OpenLoco.Dat
{
	public class PaletteMap
	{
		public PaletteMap(string filename)
			: this(Image.Load<Rgba32>(filename))
		{ }

		public PaletteMap(Image<Rgba32> img)
		{
			Palette = new (Color, byte)[256];
			for (var y = 0; y < img.Height; ++y)
			{
				for (var x = 0; x < img.Width; ++x)
				{
					var index = (byte)((y * img.Height) + x);
					Palette[index] = (img[x, y], index);
				}
			}
		}

		public PaletteMap(Color[] _palette)
		{
			_ = Verify.Equals(_palette.Length, 256);
			Palette = new (Color, byte)[256];

			for (var i = 0; i < 256; ++i)
			{
				Palette[i] = (_palette[i], (byte)i);
			}
		}

		public (Color Color, byte Index)[] Palette { get; set; }

		public Color[] PaletteColours => Palette.Select(x => x.Color).ToArray();

		public (Color Color, byte Index) Transparent
			=> Palette[0];

		public (Color Color, byte Index)[] DirectXReserved
			=> Palette[1..7];

		public (Color Color, byte Index)[] PrimaryRemapColours
			=> [.. Palette[7..10], .. Palette[246..255]];

		public (Color Color, byte Index)[] UnkReserved
			=> Palette[154..166];

		public (Color Color, byte Index)[] SecondaryRemapColours
			=> Palette[202..214];

		public (Color Color, byte Index) ChunkedTransparent
			=> Palette[255];

		public (Color Color, byte Index)[] ValidColours
			=> [.. Palette[10..154], .. Palette[166..202], .. Palette[214..246]];

		public (Color Color, byte Index)[] ReservedColours
			=> [Transparent, .. DirectXReserved, .. PrimaryRemapColours, .. UnkReserved, .. SecondaryRemapColours, ChunkedTransparent];

		public byte[] ConvertRgb32ImageToG1Data(Image<Rgba32> img)
		{
			var pixels = img.Width * img.Height;
			var bytes = new byte[pixels];

			for (var y = 0; y < img.Height; ++y)
			{
				for (var x = 0; x < img.Width; ++x)
				{
					var index = (y * img.Width) + x;
					bytes[index] = ColorToPaletteIndex(img[x, y]);
				}
			}

			return bytes;
		}

		public Image<Rgba32>? ConvertG1ToRgb32Bitmap(G1Element32 g1Element)
		{
			if (g1Element.Flags.HasFlag(G1ElementFlags.DuplicatePrevious))
			{
				return null;
			}

			var image = new Image<Rgba32>(g1Element.Width, g1Element.Height);

			for (var y = 0; y < g1Element.Height; y++)
			{
				for (var x = 0; x < g1Element.Width; x++)
				{
					var index = (y * g1Element.Width) + x;
					var paletteIndex = g1Element.ImageData[index];

					image[x, y] = paletteIndex == 0 && g1Element.Flags.HasFlag(G1ElementFlags.HasTransparency)
						? Color.Transparent
						: Palette[paletteIndex].Color;
				}
			}

			return image;
		}

		byte ColorToPaletteIndex(Color c)
		{
			var reserved = ReservedColours.Where(cc => cc.Color == c);
			if (reserved.Any())
			{
				return reserved.First().Index;
			}

			return ValidColours.MinBy(vc => DistanceSquared(c, vc.Color)).Index;
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
