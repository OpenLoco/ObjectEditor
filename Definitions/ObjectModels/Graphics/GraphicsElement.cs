using Definitions.ObjectModels.Graphics.Dithering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Graphics;

/// <summary>
/// The uniform in-memory representation of an image used throughout the editor. It is a superset of
/// every image format and property required: it carries the underlying pixel storage and the per-image
/// metadata (flags, dimensions, X/Y offsets, zoom offset, name, image-table index and raw serialised
/// bytes). <see cref="Dat.Types.DatG1Element32"/> is the DAT-layer DTO this converts to/from.
/// <para />
/// The underlying pixel storage is either an <see cref="IndexedImageFrame{Rgba32}"/> (byte indices into
/// the shared 256-color palette, for palette-indexed images) or a decoded <see cref="Image{Rgba32}"/>
/// (for <c>Bgr24</c> images), and converts between the two on demand. The raw serialised
/// <see cref="ImageData"/> bytes are kept so the object format round-trips exactly and can still be
/// serialised to JSON (the pixel caches are <c>[JsonIgnore]</c>).
/// <para />
/// The <see cref="PaletteMap"/> is a process-wide singleton (<see cref="PaletteMapLoader.Current"/>),
/// shared across the whole application, and is supplied as a parameter where conversions need it.
/// </summary>
public sealed class GraphicsElement : IDisposable
{
	bool isDisposed;

	GraphicsElement(GraphicsElementFlags flags, int width, int height, short xOffset, short yOffset, short zoomOffset, string name, int imageTableIndex, byte[] imageData, IndexedImageFrame<Rgba32>? indexed, Image<Rgba32>? rgba)
	{
		Flags = flags;
		Width = width;
		Height = height;
		XOffset = xOffset;
		YOffset = yOffset;
		ZoomOffset = zoomOffset;
		Name = name;
		ImageTableIndex = imageTableIndex;
		ImageData = imageData;
		Indexed = indexed;
		Rgba = rgba;
	}

	public GraphicsElementFlags Flags { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public short XOffset { get; set; }
	public short YOffset { get; set; }
	public short ZoomOffset { get; set; }

	[JsonIgnore]
	public string Name { get; set; } = string.Empty;

	public int ImageTableIndex { get; set; }
	public byte[] ImageData { get; set; } = [];

	public bool IsBgr24 => Flags.HasFlag(GraphicsElementFlags.IsBgr24);

	[JsonIgnore]
	public IndexedImageFrame<Rgba32>? Indexed { get; private set; }

	[JsonIgnore]
	public Image<Rgba32>? Rgba { get; private set; }

	public static GraphicsElement FromIndexed(GraphicsElementFlags flags, IndexedImageFrame<Rgba32> frame, short xOffset = 0, short yOffset = 0, short zoomOffset = 0, string name = "", int imageTableIndex = 0)
	{
		ArgumentNullException.ThrowIfNull(frame);
		return new GraphicsElement(flags, frame.Width, frame.Height, xOffset, yOffset, zoomOffset, name, imageTableIndex, [], frame, null);
	}

	public static GraphicsElement FromRgba(GraphicsElementFlags flags, Image<Rgba32> image, short xOffset = 0, short yOffset = 0, short zoomOffset = 0, string name = "", int imageTableIndex = 0, byte[]? imageData = null)
	{
		ArgumentNullException.ThrowIfNull(image);
		return new GraphicsElement(flags, image.Width, image.Height, xOffset, yOffset, zoomOffset, name, imageTableIndex, imageData ?? [], null, image);
	}

	/// <summary>Creates a <see cref="GraphicsElement"/> directly from its raw serialised bytes, deferring pixel
	/// construction until a <see cref="PaletteMap"/> is supplied. The raw bytes are preserved for exact round-trips.</summary>
	public static GraphicsElement FromData(GraphicsElementFlags flags, int width, int height, byte[] imageData, short xOffset = 0, short yOffset = 0, short zoomOffset = 0, string name = "", int imageTableIndex = 0)
	{
		ArgumentNullException.ThrowIfNull(imageData);
		return new GraphicsElement(flags, width, height, xOffset, yOffset, zoomOffset, name, imageTableIndex, imageData, null, null);
	}

	public Image<Rgba32> ToRgba(PaletteMap paletteMap, ColourSwatch primary = ColourSwatch.PrimaryRemap, ColourSwatch secondary = ColourSwatch.SecondaryRemap)
	{
		ObjectDisposedException.ThrowIf(isDisposed, this);

		// Non-default remap swatches are only used for view-only recolour previews. Never cache those
		// decodes: the cached image is owned/shared by this element and must always reflect the default
		// remap view so it can't be influenced by (or feed back into) a transient preview.
		if (primary != ColourSwatch.PrimaryRemap || secondary != ColourSwatch.SecondaryRemap)
		{
			return DecodeToRgba(paletteMap, primary, secondary);
		}

		if (Rgba != null)
		{
			return Rgba;
		}

		if (Indexed == null)
		{
			throw new InvalidOperationException("GraphicsElement has neither an indexed frame nor an RGBA image.");
		}

		Rgba = paletteMap.ConvertIndexedImageToRgba32Bitmap(Indexed, Flags, primary, secondary);
		return Rgba;
	}

	/// <summary>Decodes the element to RGBA applying the given remap swatches, always returning a fresh,
	/// caller-owned image that is never cached on the element. Use this for view-only recolour previews so
	/// the result reflects the current swatches every time and never affects the element's saved palette data.</summary>
	public Image<Rgba32> DecodeToRgba(PaletteMap paletteMap, ColourSwatch primary, ColourSwatch secondary)
	{
		ObjectDisposedException.ThrowIf(isDisposed, this);
		ArgumentNullException.ThrowIfNull(paletteMap);

		if (Indexed != null)
		{
			return paletteMap.ConvertIndexedImageToRgba32Bitmap(Indexed, Flags, primary, secondary);
		}

		if (Rgba != null)
		{
			// RGBA-native element: there is no palette remap to apply, so return a clone of the source.
			return Rgba.Clone(_ => { });
		}

		throw new InvalidOperationException("GraphicsElement has neither an indexed frame nor an RGBA image.");
	}

	public IndexedImageFrame<Rgba32> ToIndexed(PaletteMap paletteMap, DitheringMethod? ditheringMethod = null)
	{
		ObjectDisposedException.ThrowIf(isDisposed, this);

		if (Indexed != null)
		{
			return Indexed;
		}

		if (Rgba == null)
		{
			throw new InvalidOperationException("GraphicsElement has neither an indexed frame nor an RGBA image.");
		}

		Indexed = ditheringMethod.HasValue && ditheringMethod.Value != DitheringMethod.None && !IsBgr24
			? paletteMap.ConvertRgba32ImageToIndexedImageDithering(Rgba, Flags, ditheringMethod.Value)
			: paletteMap.ConvertRgba32ImageToIndexedImage(Rgba, Flags);

		return Indexed;
	}

	public byte[] ToG1Data(PaletteMap paletteMap, DitheringMethod? ditheringMethod = null)
	{
		ObjectDisposedException.ThrowIf(isDisposed, this);

		// Authoritative raw bytes take priority so loaded/imported images round-trip byte-for-byte.
		if (ImageData != null && ImageData.Length != 0)
		{
			return ImageData;
		}

		return IsBgr24 || Indexed == null
			? paletteMap.ConvertRgba32ImageToG1Data(ToRgba(paletteMap), Flags, ditheringMethod)
			: paletteMap.ConvertIndexedImageToG1Data(Indexed);
	}

	public void Dispose()
	{
		if (isDisposed)
		{
			return;
		}

		isDisposed = true;
		Indexed?.Dispose();
		Indexed = null;
		Rgba?.Dispose();
		Rgba = null;
	}

	/// <summary>Returns a clone of <paramref name="image"/> cropped to the bounding box of all
	/// non‑transparent pixels, using the same algorithm as the GUI's CropImage command.</summary>
	public static Image<Rgba32> TrimImage(Image<Rgba32> image)
	{
		ArgumentNullException.ThrowIfNull(image);

		var cropRegion = GraphicsElementOperations.FindCropRegion(image);

		if (cropRegion.Width <= 0 || cropRegion.Height <= 0)
		{
			// Fully transparent image – return a 1×1 transparent pixel.
			return image.Clone(i => i.Crop(new Rectangle(0, 0, 1, 1)));
		}

		return image.Clone(i => i.Crop(cropRegion));
	}

	/// <summary>Returns an <see cref="IndexedImageFrame{Rgba32}"/> cropped to the bounding box of all
	/// non‑transparent (non-zero) palette-index pixels, sharing the original frame's palette.</summary>
	public static IndexedImageFrame<Rgba32> TrimImage(IndexedImageFrame<Rgba32> frame)
	{
		ArgumentNullException.ThrowIfNull(frame);

		var cropRegion = GraphicsElementOperations.FindCropRegion(frame);

		if (cropRegion.Width <= 0 || cropRegion.Height <= 0)
		{
			// Fully transparent image – return a 1×1 transparent pixel.
			return new IndexedImageFrame<Rgba32>(frame.Configuration, 1, 1, frame.Palette);
		}

		var cropped = new IndexedImageFrame<Rgba32>(frame.Configuration, cropRegion.Width, cropRegion.Height, frame.Palette);
		for (var y = 0; y < cropRegion.Height; ++y)
		{
			var src = frame.DangerousGetRowSpan(cropRegion.Top + y);
			var dst = cropped.GetWritablePixelRowSpanUnsafe(y);
			for (var x = 0; x < cropRegion.Width; ++x)
			{
				dst[x] = src[cropRegion.Left + x];
			}
		}

		return cropped;
	}

	/// <summary>Trims this image in its native representation (RGBA or palette-indexed frame, whichever is
	/// present) and returns an RGBA clone of the result. Use this when the caller needs an
	/// <see cref="Image{Rgba32}"/> (e.g. to encode to PNG) without decoding the whole image up front.</summary>
	public Image<Rgba32> TrimmedToRgba(PaletteMap paletteMap, ColourSwatch primary = ColourSwatch.PrimaryRemap, ColourSwatch secondary = ColourSwatch.SecondaryRemap)
	{
		ObjectDisposedException.ThrowIf(isDisposed, this);
		ArgumentNullException.ThrowIfNull(paletteMap);

		if (Rgba != null)
		{
			return TrimImage(Rgba);
		}

		if (Indexed != null)
		{
			using var cropped = TrimImage(Indexed);
			return paletteMap.ConvertIndexedImageToRgba32Bitmap(cropped, Flags, primary, secondary);
		}

		throw new InvalidOperationException("GraphicsElement has neither an RGBA image nor an indexed frame to trim.");
	}

	/// <summary>Replaces the decoded pixel data in-place with a new RGBA image, discarding any cached
	/// palette bytes and sized metadata. Used by crop/transform operations that mutate the image.</summary>
	public void ReplaceDecoded(Image<Rgba32>? rgba, IndexedImageFrame<Rgba32>? indexed)
	{
		ObjectDisposedException.ThrowIf(isDisposed, this);

		Indexed?.Dispose();
		Indexed = indexed;
		Rgba?.Dispose();
		Rgba = rgba;

		if (rgba != null)
		{
			Width = rgba.Width;
			Height = rgba.Height;
		}

		ImageData = [];
	}

	/// <summary>Transfers ownership of the source element's decoded pixel frames into this element,
	/// replacing this element's existing decoded data. The source relinquishes its frames (its Rgba/Indexed
	/// become null), so this element and the source never share — and hence never double-dispose — the same
	/// <see cref="Image{Rgba32}"/> or <see cref="IndexedImageFrame{Rgba32}"/>.</summary>
	public void AdoptDecodedFrom(GraphicsElement source)
	{
		ObjectDisposedException.ThrowIf(isDisposed, this);
		ArgumentNullException.ThrowIfNull(source);

		var rgba = source.Rgba;
		var indexed = source.Indexed;

		// Relinquish ownership on the source before adopting, so a later Dispose of the source can't free
		// the frames this element now owns.
		source.Rgba = null;
		source.Indexed = null;

		ReplaceDecoded(rgba, indexed);
	}
}
