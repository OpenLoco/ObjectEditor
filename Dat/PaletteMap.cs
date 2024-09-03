using SkiaSharp;
using Zenith.Core;

namespace OpenLoco.Dat
{
	public class PaletteMap
	{
		public PaletteMap(string filename)
			: this(SKBitmap.Decode(filename))
		{ }

		public PaletteMap(SKBitmap img)
		{
			Palette = new (SKColor, byte)[256];
			for (var y = 0; y < img.Height; ++y)
			{
				for (var x = 0; x < img.Width; ++x)
				{
					var index = (byte)((y * img.Height) + x);
					Palette[index] = (img.GetPixel(x, y), index);
				}
			}
		}

		public PaletteMap(SKColor[] _palette)
		{
			_ = Verify.Equals(_palette.Length, 256);
			Palette = new (SKColor, byte)[256];

			for (var i = 0; i < 256; ++i)
			{
				Palette[i] = (_palette[i], (byte)i);
			}
		}

		public (SKColor Color, byte Index)[] Palette { get; set; }

		public SKColor[] PaletteColours => Palette.Select(x => x.Color).ToArray();

		public (SKColor Color, byte Index) Transparent
			=> Palette[0];
		public (SKColor Color, byte Index)[] DirectXReserved
			=> Palette[1..7];
		public (SKColor Color, byte Index)[] PrimaryRemapColours
			=> [.. Palette[7..10], .. Palette[246..255]];

		public (SKColor Color, byte Index)[] UnkReserved
			=> Palette[154..166];

		public (SKColor Color, byte Index)[] SecondaryRemapColours
			=> Palette[202..214];

		public (SKColor Color, byte Index) ChunkedTransparent
			=> Palette[255];

		public (SKColor Color, byte Index)[] ValidColours
			=> [.. Palette[10..154], .. Palette[166..202], .. Palette[214..246]];

		public (SKColor Color, byte Index)[] ReservedColours
			=> [Transparent, .. DirectXReserved, .. PrimaryRemapColours, .. UnkReserved, .. SecondaryRemapColours, ChunkedTransparent];
	}
}
