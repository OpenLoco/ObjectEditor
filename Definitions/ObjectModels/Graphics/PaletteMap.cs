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
		ArgumentOutOfRangeException.ThrowIfNotEqual(16, img.Width);
		ArgumentOutOfRangeException.ThrowIfNotEqual(16, img.Height);

		var pal = new (Color, byte)[img.Width * img.Height];
		for (var y = 0; y < img.Height; ++y)
		{
			for (var x = 0; x < img.Width; ++x)
			{
				var index = (byte)((y * img.Width) + x);
				pal[index] = (Color.FromPixel(img[x, y]), index);
			}
		}

		Palette = pal;
	}

	public PaletteMap(Color[] _palette)
	{
		ArgumentOutOfRangeException.ThrowIfNotEqual(256, _palette.Length);
		var pal = new (Color, byte)[_palette.Length];

		for (var i = 0; i < _palette.Length; ++i)
		{
			pal[i] = (_palette[i], (byte)i);
		}

		Palette = pal;
	}

	private (Color Color, byte Index)[] _palette = [];
	private readonly int[] _primaryRemapSlot = new int[256];
	private readonly int[] _secondaryRemapSlot = new int[256];
	private (Color Color, byte Index)[] _primaryRemap = [];
	private (Color Color, byte Index)[] _secondaryRemap = [];
	private (Color Color, byte Index)[] _validColours = [];
	private (Color Color, byte Index)[] _reservedColours = [];
	private (Color Color, byte Index)[] _textRendering = [];
	private readonly Dictionary<Rgba32, byte> _reservedColorLookup = [];
	private (byte R, byte G, byte B, byte Index)[] _validColorBytes = [];

	private void InitializeLookups()
	{
		_textRendering = _palette.Length >= 7 ? _palette[1..7] : [];
		_primaryRemap = _palette.Length >= 255 ? [.. _palette[7..10], .. _palette[246..255]] : [];
		_secondaryRemap = _palette.Length >= 214 ? _palette[202..214] : [];
		_validColours = _palette.Length >= 246 ? [.. _palette[10..202], .. _palette[214..246]] : [];
		_reservedColours = [Transparent, .. _textRendering, .. _primaryRemap, .. _secondaryRemap, ChunkedTransparent];

		Array.Fill(_primaryRemapSlot, -1);
		Array.Fill(_secondaryRemapSlot, -1);

		for (var i = 0; i < _primaryRemap.Length; i++)
		{
			_primaryRemapSlot[_primaryRemap[i].Index] = i;
		}

		for (var i = 0; i < _secondaryRemap.Length; i++)
		{
			_secondaryRemapSlot[_secondaryRemap[i].Index] = i;
		}

		_reservedColorLookup.Clear();
		foreach (var rc in _reservedColours)
		{
			_reservedColorLookup[rc.Color.ToPixel<Rgba32>()] = rc.Index;
		}

		_validColorBytes = new (byte R, byte G, byte B, byte Index)[_validColours.Length];
		for (var i = 0; i < _validColours.Length; i++)
		{
			var p = _validColours[i].Color.ToPixel<Rgba32>();
			_validColorBytes[i] = (p.R, p.G, p.B, _validColours[i].Index);
		}
	}

	public (Color Color, byte Index)[] Palette
	{
		get => _palette;
		set
		{
			_palette = value;
			InitializeLookups();
		}
	}

	public static (Color Color, byte Index) Transparent
		=> (TransparentPixel, 0); //Palette[0];

	public static readonly Color TransparentPixel = Color.FromPixel(new Rgba32(0, 0, 0, 0));

	public (Color Color, byte Index)[] TextRendering => _textRendering;

	public (Color Color, byte Index)[] PrimaryRemap => _primaryRemap;

	public (Color Color, byte Index)[] SecondaryRemap => _secondaryRemap;

	public (Color Color, byte Index) ChunkedTransparent
		=> Palette.Length > 255 ? Palette[255] : default;

	public (Color Color, byte Index)[] ValidColours => _validColours;

	public (Color Color, byte Index)[] ReservedColours => _reservedColours;

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

	/// <summary>Creates a zero-initialised indexed frame (all indices 0 = transparent) with the shared
	/// 256-colour <see cref="Palette"/>. Used to build placeholder/transparent palette images.</summary>
	public IndexedImageFrame<Rgba32> CreateIndexedImageFrame(int width, int height)
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
				image[x, y] = DecodeIndexToColour(paletteIndex, flags, primary, secondary).ToPixel<Rgba32>();
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
		if (paletteIndex == 0 && flags.HasFlag(GraphicsElementFlags.HasTransparency))
		{
			return Transparent.Color;
		}

		var slotP = _primaryRemapSlot[paletteIndex];
		if (slotP >= 0)
		{
			var swatch = GetRemapSwatchFromName(primary);
			if (swatch != null && (uint)slotP < (uint)swatch.Length)
			{
				return swatch[slotP].Color;
			}
		}

		var slotS = _secondaryRemapSlot[paletteIndex];
		if (slotS >= 0)
		{
			var swatch = GetRemapSwatchFromName(secondary);
			if (swatch != null && (uint)slotS < (uint)swatch.Length)
			{
				return swatch[slotS].Color;
			}
		}

		return Palette[paletteIndex].Color;
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
					image[x, y] = DecodeIndexToColour(paletteIndex, flags, primary, secondary).ToPixel<Rgba32>();
					index++;
				}
			}
		}

		return true;
	}

	byte ColorToPaletteIndex(Color c)
	{
		var p = c.ToPixel<Rgba32>();
		if (p.A == 0)
		{
			return 0;
		}

		if (_reservedColorLookup.TryGetValue(p, out var reservedIdx))
		{
			return reservedIdx;
		}

		var bestDist = int.MaxValue;
		byte bestIdx = 0;
		int pr = p.R, pg = p.G, pb = p.B;
		for (var i = 0; i < _validColorBytes.Length; i++)
		{
			ref readonly var vc = ref _validColorBytes[i];
			var dr = pr - vc.R;
			var dg = pg - vc.G;
			var db = pb - vc.B;
			var dist = (dr * dr) + (dg * dg) + (db * db);
			if (dist < bestDist)
			{
				bestDist = dist;
				bestIdx = vc.Index;
				if (dist == 0)
				{
					break;
				}
			}
		}

		return bestIdx;
	}
}
