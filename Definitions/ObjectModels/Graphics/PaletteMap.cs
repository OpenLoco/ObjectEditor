using Definitions.ObjectModels.Graphics.Dithering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Definitions.ObjectModels.Graphics;

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
				Palette[index] = (Color.FromPixel(img[x, y]), index);
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

	public static readonly Color TransparentPixel = Color.FromPixel(new Rgba32(0, 0, 0, 0));

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
		=> [.. Palette[230..240], .. Palette[243..245]];

	#endregion

	#region Misc Usable Colours

	public (Color Color, byte Index)[] MiscGrey
		=> [Palette[226], Palette[240], Palette[241], Palette[242]];
	public (Color Color, byte Index)[] MiscYellow
		=> [Palette[227], Palette[228], Palette[229]];

	#endregion

	public byte[] ConvertRgba32ImageToG1Data(Image<Rgba32> img, GraphicsElementFlags flags, DitheringMethod? method = null)
	{
		// BGR24 stores raw RGB triplets rather than palette indices, so no indexing/dithering applies.
		if (flags.HasFlag(GraphicsElementFlags.IsBgr24))
		{
			var bytes = new byte[img.Width * img.Height * 3];
			var index = 0;
			for (var y = 0; y < img.Height; ++y)
			{
				for (var x = 0; x < img.Width; ++x)
				{
					var pixel = img[x, y];
					bytes[index++] = pixel.B;
					bytes[index++] = pixel.G;
					bytes[index++] = pixel.R;
				}
			}

			return bytes;
		}

		// If a dithering method is selected, use the dithered pipeline.
		if (method.HasValue && method.Value != DitheringMethod.None)
		{
			return ConvertRgba32ImageToG1DataWithDithering(img, flags, method.Value);
		}

		// Otherwise convert to the shared IndexedImageFrame (which preserves reserved/company colours)
		// and extract the G1 palette-index bytes from it.
		using var indexed = ConvertRgba32ImageToIndexedImage(img, flags);
		return ConvertIndexedImageToG1Data(indexed);
	}

	ReadOnlyMemory<Rgba32> CreatePaletteMemory()
		=> new ReadOnlyMemory<Rgba32>(Palette.Select(p => p.Color.ToPixel<Rgba32>()).ToArray());

	IndexedImageFrame<Rgba32> CreateIndexedImageFrame(Image<Rgba32> img)
		=> CreateIndexedImageFrame(img.Width, img.Height);

	IndexedImageFrame<Rgba32> CreateIndexedImageFrame(int width, int height)
		=> new IndexedImageFrame<Rgba32>(Configuration.Default, width, height, CreatePaletteMemory());

	/// <summary>
	/// Creates an <see cref="IndexedImageFrame{Rgba32}"/> backed by the shared 256-colour
	/// <see cref="Palette"/>. Each pixel is converted to its palette index via
	/// <see cref="ColorToPaletteIndex"/>, which matches reserved colours (transparent, text rendering,
	/// primary/secondary remap and chunked transparent) exactly and preserves them, falling back to the
	/// nearest valid colour for everything else. Dithering and further palette operations are layered on
	/// top of this indexed image.
	/// </summary>
	public IndexedImageFrame<Rgba32> ConvertRgba32ImageToIndexedImage(Image<Rgba32> img, GraphicsElementFlags flags)
	{
		var frame = CreateIndexedImageFrame(img);

		for (var y = 0; y < img.Height; ++y)
		{
			var row = frame.GetWritablePixelRowSpanUnsafe(y);
			for (var x = 0; x < img.Width; ++x)
			{
				var pixel = img[x, y];
				// Binarize alpha as the original conversion did: any translucency becomes fully transparent.
				pixel.A = pixel.A == 255 ? (byte)255 : (byte)0;
				row[x] = ColorToPaletteIndex(Color.FromPixel(pixel));
			}
		}

		return frame;
	}

	/// <summary>Extracts 8-bit G1 palette-index bytes from an <see cref="IndexedImageFrame{Rgba32}"/>.</summary>
	public byte[] ConvertIndexedImageToG1Data(IndexedImageFrame<Rgba32> frame)
	{
		var bytes = new byte[frame.Width * frame.Height];
		for (var y = 0; y < frame.Height; ++y)
		{
			var row = frame.DangerousGetRowSpan(y);
			for (var x = 0; x < frame.Width; ++x)
			{
				bytes[(y * frame.Width) + x] = row[x];
			}
		}

		return bytes;
	}

	/// <summary>Decodes a palette-indexed <see cref="IndexedImageFrame{Rgba32}"/> into an RGBA image.</summary>
	/// <param name="frame">The indexed frame to decode (indices into <see cref="Palette"/>).</param>
	/// <param name="flags">The element flags (used to interpret transparency and remap).</param>
	/// <param name="primary">The primary remap swatch used to colour primary-remapped indices.</param>
	/// <param name="secondary">The secondary remap swatch used to colour secondary-remapped indices.</param>
	public Image<Rgba32> ConvertIndexedImageToRgba32Bitmap(IndexedImageFrame<Rgba32> frame, GraphicsElementFlags flags, ColourSwatch primary, ColourSwatch secondary)
	{
		var image = new Image<Rgba32>(frame.Width, frame.Height);

		for (var y = 0; y < frame.Height; y++)
		{
			var row = frame.DangerousGetRowSpan(y);
			for (var x = 0; x < frame.Width; x++)
			{
				var paletteIndex = row[x];
				Color? colour = null;

				if (paletteIndex == 0 && flags.HasFlag(GraphicsElementFlags.HasTransparency))
				{
					colour = Transparent.Color;
				}
				else if (PrimaryRemap.Index().SingleOrDefault(p => p.Item.Index == paletteIndex) is (int, (Color, byte)) itemP && itemP.Index != 0)
				{
					var swatch = GetRemapSwatchFromName(primary);
					if (swatch != null)
					{
						colour = swatch[itemP.Index].Color;
					}
				}
				else if (SecondaryRemap.Index().SingleOrDefault(s => s.Item.Index == paletteIndex) is (int, (Color, byte)) itemS && itemS.Index != 0)
				{
					var swatch = GetRemapSwatchFromName(secondary);
					if (swatch != null)
					{
						colour = swatch[itemS.Index].Color;
					}
				}

				image[x, y] = (colour ?? Palette[paletteIndex].Color).ToPixel<Rgba32>();
			}
		}

		return image;
	}

	/// <summary>Creates an <see cref="IndexedImageFrame{Rgba32}"/> from raw palette-index bytes.
	/// The frame's byte indices are copied verbatim (preserving company/remap &amp; reserved indices). Returns false if Bgr24.</summary>
	public bool TryConvertImageDataToIndexedImage(int width, int height, GraphicsElementFlags flags, byte[] imageData, out IndexedImageFrame<Rgba32> frame)
	{
		frame = null!;
		if (flags.HasFlag(GraphicsElementFlags.IsBgr24))
		{
			return false;
		}

		frame = CreateIndexedImageFrame(width, height);
		var index = 0;
		for (var y = 0; y < height; y++)
		{
			var row = frame.GetWritablePixelRowSpanUnsafe(y);
			for (var x = 0; x < width; x++)
			{
				row[x] = index < imageData.Length ? imageData[index] : (byte)0;
				index++;
			}
		}

		return true;
	}

	/// <summary>Creates a decoded <see cref="Image{Rgba32}"/> directly from raw image-data bytes
	/// (handles both indexed and Bgr24 data). Used when materialising a view from raw DAT bytes.</summary>
	public Image<Rgba32> ConvertImageDataToRgba32Bitmap(int width, int height, GraphicsElementFlags flags, byte[] imageData, ColourSwatch primary = ColourSwatch.PrimaryRemap, ColourSwatch secondary = ColourSwatch.SecondaryRemap)
	{
		var image = new Image<Rgba32>(width, height);
		var index = 0;

		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				if (flags.HasFlag(GraphicsElementFlags.IsBgr24))
				{
					if (index + 2 >= imageData.Length)
					{
						break;
					}

					var b = imageData[index++];
					var g = imageData[index++];
					var r = imageData[index++];
					image[x, y] = new Rgba32(r, g, b);
				}
				else
				{
					var paletteIndex = index < imageData.Length ? imageData[index] : (byte)0;
					var colour = DecodeIndexToColour(paletteIndex, flags, primary, secondary);
					image[x, y] = colour.ToPixel<Rgba32>();
					index++;
				}
			}
		}

		return image;
	}

	Color DecodeIndexToColour(byte paletteIndex, GraphicsElementFlags flags, ColourSwatch primary, ColourSwatch secondary)
	{
		Color? colour = null;

		if (paletteIndex == 0 && flags.HasFlag(GraphicsElementFlags.HasTransparency))
		{
			colour = Transparent.Color;
		}
		else if (PrimaryRemap.Index().SingleOrDefault(p => p.Item.Index == paletteIndex) is (int, (Color, byte)) itemP && itemP.Index != 0)
		{
			var swatch = GetRemapSwatchFromName(primary);
			if (swatch != null)
			{
				colour = swatch[itemP.Index].Color;
			}
		}
		else if (SecondaryRemap.Index().SingleOrDefault(s => s.Item.Index == paletteIndex) is (int, (Color, byte)) itemS && itemS.Index != 0)
		{
			var swatch = GetRemapSwatchFromName(secondary);
			if (swatch != null)
			{
				colour = swatch[itemS.Index].Color;
			}
		}

		return colour ?? Palette[paletteIndex].Color;
	}

	/// <summary>
	/// Converts an RGBA32 image to an <see cref="IndexedImageFrame{Rgba32}"/> using mask-aware multi-channel
	/// dithering (keeps company colours &amp; other reserved colours preserved).
	/// </summary>
	public IndexedImageFrame<Rgba32> ConvertRgba32ImageToIndexedImageDithering(Image<Rgba32> img, GraphicsElementFlags flags, DitheringMethod method = DitheringMethod.FloydSteinberg)
	{
		// Bgr24 is not indexed; callers should not reach here for Bgr24 elements.
		if (flags.HasFlag(GraphicsElementFlags.IsBgr24))
		{
			throw new InvalidOperationException("Dithering does not apply to Bgr24 images.");
		}

		var frame = CreateIndexedImageFrame(img);
		MaskedDitherer.Dither(img, this, frame, method);
		return frame;
	}

	/// <summary>Converts an RGBA32 image to G1 palette-indexed data via a dithering pipeline.</summary>
	public byte[] ConvertRgba32ImageToG1DataWithDithering(Image<Rgba32> img, GraphicsElementFlags flags, DitheringMethod method = DitheringMethod.FloydSteinberg)
	{
		// BGR24 images are copied directly, no dithering needed.
		if (flags.HasFlag(GraphicsElementFlags.IsBgr24))
		{
			var bytes = new byte[img.Width * img.Height * 3];
			var index = 0;
			for (var y = 0; y < img.Height; y++)
			{
				for (var x = 0; x < img.Width; x++)
				{
					var pixel = img[x, y];
					bytes[index++] = pixel.B;
					bytes[index++] = pixel.G;
					bytes[index++] = pixel.R;
				}
			}

			return bytes;
		}

		// Dither into a shared IndexedImageFrame, then extract the G1 palette-index bytes.
		// The ditherer runs independent passes per category (valid / primary / secondary) so
		// company colours (primary & secondary remap) and other reserved colours are preserved.
		using var frame = CreateIndexedImageFrame(img);
		MaskedDitherer.Dither(img, this, frame, method);
		return ConvertIndexedImageToG1Data(frame);
	}

	public (Color Color, byte Index)[]? GetRemapSwatchFromName(ColourSwatch swatch)
		=> swatch switch
		{
			ColourSwatch.Black => Black,
			ColourSwatch.Bronze => Bronze,
			ColourSwatch.Copper => Copper,
			ColourSwatch.Yellow => Yellow,
			ColourSwatch.Rose => Rose,
			ColourSwatch.GrassGreen => GrassGreen,
			ColourSwatch.AvocadoGreen => AvocadoGreen,
			ColourSwatch.Green => Green,
			ColourSwatch.Brass => Brass,
			ColourSwatch.Lavender => Lavender,
			ColourSwatch.Blue => Blue,
			ColourSwatch.SeaGreen => SeaGreen,
			ColourSwatch.Purple => Purple,
			ColourSwatch.Red => Red,
			ColourSwatch.Orange => Orange,
			ColourSwatch.Teal => Teal,
			ColourSwatch.Brown => Brown,
			ColourSwatch.Amber => Amber,
			ColourSwatch.PrimaryRemap => PrimaryRemap,
			ColourSwatch.SecondaryRemap => SecondaryRemap,
			_ => default,
		};

	public bool TryConvertG1ToRgba32Bitmap(int width, int height, GraphicsElementFlags flags, byte[] imageData, ColourSwatch primary, ColourSwatch secondary, out Image<Rgba32>? image)
	{
		image = new Image<Rgba32>(width, height);

		var index = 0;
		for (var y = 0; y < height; y++)
		{
			for (var x = 0; x < width; x++)
			{
				if (flags.HasFlag(GraphicsElementFlags.IsBgr24))
				{
					if (index >= imageData.Length)
					{
						// malformed image - didn't have enough bytes to cover the full dimensions
						// steam's g1.dat index 304 (the default palette) has this issue. 236x16 but should be 236x1 since it only has 236*3=708 bytes of data
						break;
					}

					var b = imageData[index++];
					var g = imageData[index++];
					var r = imageData[index++];
					image[x, y] = new Rgba32(r, g, b);
				}
				else
				{
					var paletteIndex = imageData[index];
					Color? colour = null;

					if (SecondaryRemap.Any(x => x.Index == paletteIndex))
					{
						//Debugger.Break();
					}

					if (paletteIndex == 0 && flags.HasFlag(GraphicsElementFlags.HasTransparency))
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

					image[x, y] = (colour ?? Palette[paletteIndex].Color).ToPixel<Rgba32>();
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
