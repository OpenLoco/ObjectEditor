using Dat.Types;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Dat;

public enum ColourRemapSwatch
{
	Amber,
	AvocadoGreen,
	Black,
	Blue,
	Brass,
	Bronze,
	Brown,
	Copper,
	GrassGreen,
	Green,
	Lavender,
	Orange,
	Purple,
	Red,
	Rose,
	SeaGreen,
	Teal,
	Yellow,
	//MiscGrey,
	//MiscYellow,
	PrimaryRemap,
	SecondaryRemap,
}

public class PaletteMap
{
	public PaletteMap(string filename)
		: this(Image.Load<Rgba32>(filename))
	{ }

	public PaletteMap(Image<Rgba32> img)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(16, img.Height);
		ArgumentOutOfRangeException.ThrowIfNotEqual(16, img.Height);

		Palette = new (Color, byte)[img.Width * img.Height];
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
		ArgumentOutOfRangeException.ThrowIfNotEqual(256, _palette.Length);
		Palette = new (Color, byte)[_palette.Length];

		for (var i = 0; i < _palette.Length; ++i)
		{
			Palette[i] = (_palette[i], (byte)i);
		}
	}

	public (Color Color, byte Index)[] Palette { get; set; }

	public static (Color Color, byte Index) Transparent
		=> (TransparentPixel, 0); //Palette[0];

	public static readonly Color TransparentPixel = Color.FromRgba(0, 0, 0, 0);

	public (Color Color, byte Index)[] TextRendering
		=> Palette[1..7];

	public (Color Color, byte Index)[] PrimaryRemap
		=> [.. Palette[7..10], .. Palette[246..255]];

	public (Color Color, byte Index)[] SecondaryRemap
		=> Palette[202..214];

	public (Color Color, byte Index) ChunkedTransparent
		=> Palette[255];

	public (Color Color, byte Index)[] ValidColours
		=> [.. Palette[10..202], .. Palette[214..246]];

	public (Color Color, byte Index)[] ReservedColours
		=> [Transparent, .. TextRendering, .. PrimaryRemap, .. SecondaryRemap, ChunkedTransparent];

	#region Colour Swatches

	public (Color Color, byte Index)[] Black
		=> Palette[10..22];
	public (Color Color, byte Index)[] Bronze
		=> Palette[22..34];
	public (Color Color, byte Index)[] Copper
		=> Palette[34..46];
	public (Color Color, byte Index)[] Yellow
		=> Palette[46..58];
	public (Color Color, byte Index)[] Rose
		=> Palette[58..70];
	public (Color Color, byte Index)[] GrassGreen
		=> Palette[70..82];
	public (Color Color, byte Index)[] AvocadoGreen
		=> Palette[82..94];
	public (Color Color, byte Index)[] Green
		=> Palette[94..106];
	public (Color Color, byte Index)[] Brass
		=> Palette[106..118];
	public (Color Color, byte Index)[] Lavender
		=> Palette[118..130];
	public (Color Color, byte Index)[] Blue
		=> Palette[130..142];
	public (Color Color, byte Index)[] SeaGreen
		=> Palette[142..154];
	public (Color Color, byte Index)[] Purple
		=> Palette[154..166];
	public (Color Color, byte Index)[] Red
		=> Palette[166..178];
	public (Color Color, byte Index)[] Orange
		=> Palette[178..190];
	public (Color Color, byte Index)[] Teal
		=> Palette[190..202];
	public (Color Color, byte Index)[] Brown
		=> Palette[214..226];
	public (Color Color, byte Index)[] Amber
		=> [.. Palette[230..240], .. Palette[244..246]];

	#endregion

	#region Misc Usable Colours

	public (Color Color, byte Index)[] MiscGrey
		=> [Palette[226], Palette[240], Palette[241], Palette[242]];
	public (Color Color, byte Index)[] MiscYellow
		=> [Palette[227], Palette[228], Palette[229]];

	#endregion

	public byte[] ConvertRgba32ImageToG1Data(Image<Rgba32> img, G1ElementFlags flags)
	{
		var pixels = img.Width * img.Height;
		var isBgr = flags.HasFlag(G1ElementFlags.IsBgr24);
		var bytes = new byte[pixels * (isBgr ? 3 : 1)];

		var index = 0;
		for (var y = 0; y < img.Height; ++y)
		{
			for (var x = 0; x < img.Width; ++x)
			{
				if (isBgr)
				{
					var pixel = img[x, y];
					bytes[index++] = pixel.B;
					bytes[index++] = pixel.G;
					bytes[index++] = pixel.R;
				}
				else
				{
					index = (y * img.Width) + x;
					bytes[index] = ColorToPaletteIndex(img[x, y]);
				}
			}
		}

		return bytes;
	}

	public (Color Color, byte Index)[]? GetRemapSwatchFromName(ColourRemapSwatch swatch)
		=> swatch switch
		{
			ColourRemapSwatch.Black => Black,
			ColourRemapSwatch.Bronze => Bronze,
			ColourRemapSwatch.Copper => Copper,
			ColourRemapSwatch.Yellow => Yellow,
			ColourRemapSwatch.Rose => Rose,
			ColourRemapSwatch.GrassGreen => GrassGreen,
			ColourRemapSwatch.AvocadoGreen => AvocadoGreen,
			ColourRemapSwatch.Green => Green,
			ColourRemapSwatch.Brass => Brass,
			ColourRemapSwatch.Lavender => Lavender,
			ColourRemapSwatch.Blue => Blue,
			ColourRemapSwatch.SeaGreen => SeaGreen,
			ColourRemapSwatch.Purple => Purple,
			ColourRemapSwatch.Red => Red,
			ColourRemapSwatch.Orange => Orange,
			ColourRemapSwatch.Teal => Teal,
			ColourRemapSwatch.Brown => Brown,
			ColourRemapSwatch.Amber => Amber,
			ColourRemapSwatch.PrimaryRemap => PrimaryRemap,
			ColourRemapSwatch.SecondaryRemap => SecondaryRemap,
			_ => default,
		};

	public bool TryConvertG1ToRgba32Bitmap(G1Element32 g1Element, ColourRemapSwatch primary, ColourRemapSwatch secondary, out Image<Rgba32>? image)
	{
		image = new Image<Rgba32>(g1Element.Width, g1Element.Height);

		var index = 0;
		for (var y = 0; y < g1Element.Height; y++)
		{
			for (var x = 0; x < g1Element.Width; x++)
			{
				if (g1Element.Flags.HasFlag(G1ElementFlags.IsBgr24))
				{
					if (index >= g1Element.ImageData.Length)
					{
						// malformed image - didn't have enough bytes to cover the full dimensions
						// steam's g1.dat index 304 (the default palette) has this issue. 236x16 but should be 236x1 since it only has 236*3=708 bytes of data
						break;
					}

					var b = g1Element.ImageData[index++];
					var g = g1Element.ImageData[index++];
					var r = g1Element.ImageData[index++];
					image[x, y] = Color.FromRgb(r, g, b);
				}
				else
				{
					var paletteIndex = g1Element.ImageData[index];
					Color? colour = null;

					if (SecondaryRemap.Any(x => x.Index == paletteIndex))
					{
						//Debugger.Break();
					}

					if (paletteIndex == 0 && g1Element.Flags.HasFlag(G1ElementFlags.HasTransparency))
					{
						colour = Transparent.Color;
					}
					else if (PrimaryRemap.Index().SingleOrDefault(x => x.Item.Index == paletteIndex) is (int, (Color, byte)) itemP && itemP.Index != 0)
					{
						var swatch = GetRemapSwatchFromName(primary);
						if (swatch != null)
						{
							colour = swatch[itemP.Index].Color;
						}
					}
					else if (SecondaryRemap.Index().SingleOrDefault(x => x.Item.Index == paletteIndex) is (int, (Color, byte)) itemS && itemS.Index != 0)
					{
						var swatch = GetRemapSwatchFromName(secondary);
						if (swatch != null)
						{
							colour = swatch[itemS.Index].Color;
						}
					}

					image[x, y] = colour ?? Palette[paletteIndex].Color;
					index++;
				}
			}
		}

		return true;
	}

	byte ColorToPaletteIndex(Color c)
	{
		if (c.ToPixel<Rgba32>().A == 0)
		{
			c = TransparentPixel;
		}

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
