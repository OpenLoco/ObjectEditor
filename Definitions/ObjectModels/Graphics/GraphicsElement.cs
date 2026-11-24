using Definitions.ObjectModels.Graphics.Dithering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Graphics;

/// <summary>
/// The uniform in-memory representation of an image used throughout the editor. It is a superset of
/// every image format and property required: it carries the underlying pixel storage and the per-image
/// metadata (flags, dimensions, X/Y offsets, zoom offset, name and image-table index).
/// <see cref="Dat.Types.DatG1Element32"/> is the DAT-layer DTO this converts to/from.
/// <para />
/// Every image is stored in exactly one native representation, which is that image's source of truth:
///  - palette-indexed images (anything that isn't <c>Bgr24</c>) are stored as an
///    <see cref="IndexedImageFrame{Rgba32}"/> (byte indices into the shared 256-colour palette),
///    preserving company/remap &amp; reserved indices losslessly. <see cref="Indexed"/> is the truth.
///  - <c>Bgr24</c> images are stored as a decoded <see cref="Image{Rgba32}"/>.
/// At least one of <see cref="Indexed"/>/<see cref="Rgba"/> is always set; <see cref="Rgba"/> is only
/// ever the source of truth for <c>Bgr24</c> images and is never set alongside an indexed frame.
/// <para />
/// The <see cref="PaletteMap"/> is a process-wide singleton (<see cref="PaletteMapLoader.Current"/>),
/// shared across the whole application, and is supplied as a parameter where conversions need it.
/// </summary>
public sealed class GraphicsElement : IDisposable
{
	bool isDisposed;

	GraphicsElement(GraphicsElementFlags flags, int width, int height, short xOffset, short yOffset, short zoomOffset, string name, int imageTableIndex, IndexedImageFrame<Rgba32>? indexed, Image<Rgba32>? rgba)
	{
		if (indexed == null == (rgba == null))
		{
			throw new ArgumentException("Exactly one of IndexedImageFrame / Image<Rgba32> must be supplied.");
		}

		Flags = flags;
		Width = width;
		Height = height;
		XOffset = xOffset;
		YOffset = yOffset;
		ZoomOffset = zoomOffset;
		Name = name;
		ImageTableIndex = imageTableIndex;
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

	/// <summary>Palette-indexed pixels - the source of truth for palette images (never null for a palette image).</summary>
	[JsonIgnore]
	public IndexedImageFrame<Rgba32>? Indexed { get; private set; }

	/// <summary>Decoded RGBA pixels - the source of truth only for <c>Bgr24</c> images (never set alongside <see cref="Indexed"/>).</summary>
	[JsonIgnore]
	public Image<Rgba32>? Rgba { get; private set; }

	public static GraphicsElement FromIndexed(GraphicsElementFlags flags, IndexedImageFrame<Rgba32> frame, short xOffset = 0, short yOffset = 0, short zoomOffset = 0, string name = "", int imageTableIndex = 0)
	{
		ArgumentNullException.ThrowIfNull(frame);
		return new GraphicsElement(flags, frame.Width, frame.Height, xOffset, yOffset, zoomOffset, name, imageTableIndex, frame, null);
	}

	/// <summary>Creates an element backed by decoded RGBA pixels. Only <c>Bgr24</c> images are stored as RGBA;
	/// a palette-format image must be converted to an indexed frame first and created with <see cref="FromIndexed"/>.</summary>
	public static GraphicsElement FromRgba(GraphicsElementFlags flags, Image<Rgba32> image, short xOffset = 0, short yOffset = 0, short zoomOffset = 0, string name = "", int imageTableIndex = 0)
	{
		ArgumentNullException.ThrowIfNull(image);
		if (!flags.HasFlag(GraphicsElementFlags.IsBgr24))
		{
			throw new ArgumentException("RGBA storage is only used for Bgr24 images; palette-format images must be stored as an IndexedImageFrame.", nameof(flags));
		}

		return new GraphicsElement(flags, image.Width, image.Height, xOffset, yOffset, zoomOffset, name, imageTableIndex, null, image);
	}

	public Image<Rgba32> ToRgba(PaletteMap paletteMap, ColourSwatch primary = ColourSwatch.PrimaryRemap, ColourSwatch secondary = ColourSwatch.SecondaryRemap)
		=> DecodeToRgba(paletteMap, primary, secondary);

	/// <summary>Decodes the element to RGBA applying the given remap swatches, returning a fresh,
	/// caller-owned image that is never recorded on the element. Use for display/previews so the result
	/// always reflects the current swatches and never affects the element's source of truth.</summary>
	public Image<Rgba32> DecodeToRgba(PaletteMap paletteMap, ColourSwatch primary, ColourSwatch secondary)
	{
		ObjectDisposedException.ThrowIf(isDisposed, this);
		ArgumentNullException.ThrowIfNull(paletteMap);

		if (Indexed != null)
		{
			return paletteMap.ConvertIndexedImageToRgba32Bitmap(Indexed, Flags, primary, secondary);
		}

		// Bgr24 source: there is no palette remap to apply, so return a clone the caller may own/dispose.
		return Rgba!.Clone(_ => { });
	}

	/// <summary>Serialises this element to its raw DAT/G1 bytes from its source of truth - palette indices
	/// for an indexed frame, raw BGR triplets for <c>Bgr24</c>. Palette images are never lossy (remap/company
	/// indices are preserved exactly by <see cref="PaletteMap.ConvertIndexedImageToG1Data"/>).</summary>
	public byte[] ToG1Data(PaletteMap paletteMap, DitheringMethod? ditheringMethod = null)
	{
		ObjectDisposedException.ThrowIf(isDisposed, this);
		ArgumentNullException.ThrowIfNull(paletteMap);

		if (Indexed != null)
		{
			return paletteMap.ConvertIndexedImageToG1Data(Indexed);
		}

		// Bgr24 only here - serialise raw RGB triplets; dithering does not apply.
		return paletteMap.ConvertRgba32ImageToG1Data(Rgba!, Flags, ditheringMethod);
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

		// Indexed (palette) frames are the source of truth - crop them natively so remap/company indices
		// are preserved, decoding only the cropped result.
		if (Indexed != null)
		{
			using var cropped = TrimImage(Indexed);
			return paletteMap.ConvertIndexedImageToRgba32Bitmap(cropped, Flags, primary, secondary);
		}

		if (Rgba != null)
		{
			return TrimImage(Rgba);
		}

		throw new InvalidOperationException("GraphicsElement has neither an indexed frame nor an RGBA image.");
	}

	/// <summary>Replaces the decoded pixel storage in-place. Exactly one of <paramref name="rgba"/> /
	/// <paramref name="indexed"/> must be supplied, matching the element's native representation.</summary>
	public void ReplaceDecoded(Image<Rgba32>? rgba, IndexedImageFrame<Rgba32>? indexed)
	{
		ObjectDisposedException.ThrowIf(isDisposed, this);
		if (rgba == null == (indexed == null))
		{
			throw new ArgumentException("Exactly one of rgba / indexed must be supplied.");
		}

		Indexed?.Dispose();
		Indexed = indexed;
		Rgba?.Dispose();
		Rgba = rgba;

		if (indexed != null)
		{
			Width = indexed.Width;
			Height = indexed.Height;
		}
		else
		{
			Width = rgba!.Width;
			Height = rgba.Height;
		}
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
