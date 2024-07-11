using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using Zenith.Core;
using Color = SixLabors.ImageSharp.Color;

namespace OpenLoco.ObjectEditor
{
	public class PaletteMap
	{
		public PaletteMap(Image<Rgb24> img)
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
			_ = Verify.Equals(_palette.Length, 255);
			Palette = new (Color, byte)[256];

			for (var i = 0; i < 256; ++i)
			{
				Palette[i] = (_palette[i], (byte)i);
			}
		}

		public (Color Color, byte Index)[] Palette { get; set; }

		public (Color Color, byte Index) Transparent
			=> Palette[0];
		public (Color Color, byte Index)[] DirectXReserved
			=> Palette[1..6];
		public (Color Color, byte Index)[] PrimaryRemapColors
			=> [.. Palette[7..9], .. Palette[246..254]];
		public (Color Color, byte Index)[] SecondaryRemapColours
			=> Palette[202..213];
		public (Color Color, byte Index) Glass
			=> Palette[47];
		public (Color Color, byte Index) ChunkedTransparent
			=> Palette[255];

		public (Color Color, byte Index)[] ValidColours
			=> [.. Palette[10..201], .. Palette[214..244]];

		public (Color Color, byte Index)[] ReservedColours
			=> [Transparent, .. DirectXReserved, .. PrimaryRemapColors, .. SecondaryRemapColours, ChunkedTransparent];
	}
}
