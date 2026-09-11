using Definitions.ObjectModels.Graphics.Dithering;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Definitions.ObjectModels.Graphics;

public static class GraphicsElementOperations
{
	/// <summary>Mutates this <see cref="GraphicsElement"/> in-place, cropping it to the bounding box of its
	/// non-transparent pixels and adjusting its X/Y offsets by the crop delta (as the element crop used to).</summary>
	public static void Crop(this GraphicsElement image, PaletteMap paletteMap)
	{
		ArgumentNullException.ThrowIfNull(image);
		ArgumentNullException.ThrowIfNull(paletteMap);

		// Palette image: crop the indexed frame natively so company/remap indices are preserved losslessly
		// (never decode to RGBA and back). TrimImage crops to the non-zero bbox, or 1x1 if fully transparent.
		if (image.Indexed != null)
		{
			var cropRegion = FindCropRegion(image.Indexed);
			var cropped = GraphicsElement.TrimImage(image.Indexed);
			image.ReplaceDecoded(null, cropped);

			if (cropRegion.Width <= 0 || cropRegion.Height <= 0)
			{
				image.XOffset = 0;
				image.YOffset = 0;
			}
			else
			{
				image.XOffset = (short)(image.XOffset + cropRegion.Left);
				image.YOffset = (short)(image.YOffset + cropRegion.Top);
			}

			return;
		}

		// Bgr24 image: crop the RGBA pixels.
		var rgba = image.Rgba!;
		var rgbaCropRegion = FindCropRegion(rgba);

		if (rgbaCropRegion.Width <= 0 || rgbaCropRegion.Height <= 0)
		{
			// fully transparent image - collapse to a 1x1 transparent pixel (same fallback the GUI uses)
			image.ReplaceDecoded(rgba.Clone(i => i.Crop(new Rectangle(0, 0, 1, 1))), null);
			image.XOffset = 0;
			image.YOffset = 0;
			return;
		}

		image.ReplaceDecoded(rgba.Clone(i => i.Crop(rgbaCropRegion)), null);
		image.XOffset = (short)(image.XOffset + rgbaCropRegion.Left);
		image.YOffset = (short)(image.YOffset + rgbaCropRegion.Top);
	}

	public static void ZeroOffsets(this GraphicsElement image)
	{
		ArgumentNullException.ThrowIfNull(image);

		image.XOffset = 0;
		image.YOffset = 0;
	}

	public static void CenterOffsets(this GraphicsElement image)
	{
		ArgumentNullException.ThrowIfNull(image);

		image.XOffset = (short)(-image.Width / 2);
		image.YOffset = (short)(-image.Height / 2);
	}

	public static void TranslateOffsets(this GraphicsElement image, short deltaX, short deltaY)
	{
		ArgumentNullException.ThrowIfNull(image);

		image.XOffset = (short)(image.XOffset + deltaX);
		image.YOffset = (short)(image.YOffset + deltaY);
	}

	public static Rectangle FindCropRegion(Image<Rgba32> image)
	{
		ArgumentNullException.ThrowIfNull(image);

		var minX = image.Width;
		var maxX = 0;
		var minY = image.Height;
		var maxY = 0;

		for (var y = 0; y < image.Height; y++)
		{
			for (var x = 0; x < image.Width; x++)
			{
				var pixel = image[x, y];

				if (pixel.A > 0)
				{
					minX = Math.Min(minX, x);
					maxX = Math.Max(maxX, x);
					minY = Math.Min(minY, y);
					maxY = Math.Max(maxY, y);
				}
			}
		}

		// Calculate the crop area. Ensure it is within image bounds.
		var width = Math.Max(0, Math.Min(maxX - minX + 1, image.Width - minX));
		var height = Math.Max(0, Math.Min(maxY - minY + 1, image.Height - minY));
		return new Rectangle(minX, minY, width, height);
	}

	/// <summary>Finds the bounding rectangle of all non-transparent (non-zero) palette-index pixels in an indexed frame.</summary>
	public static Rectangle FindCropRegion(IndexedImageFrame<Rgba32> frame)
	{
		ArgumentNullException.ThrowIfNull(frame);

		var minX = frame.Width;
		var maxX = 0;
		var minY = frame.Height;
		var maxY = 0;

		for (var y = 0; y < frame.Height; y++)
		{
			var row = frame.DangerousGetRowSpan(y);
			for (var x = 0; x < frame.Width; x++)
			{
				if (row[x] != 0)
				{
					minX = Math.Min(minX, x);
					maxX = Math.Max(maxX, x);
					minY = Math.Min(minY, y);
					maxY = Math.Max(maxY, y);
				}
			}
		}

		// Calculate the crop area. Ensure it is within image bounds.
		var width = Math.Max(0, Math.Min(maxX - minX + 1, frame.Width - minX));
		var height = Math.Max(0, Math.Min(maxY - minY + 1, frame.Height - minY));
		return new Rectangle(minX, minY, width, height);
	}

	/// <summary>Builds the in-memory <see cref="GraphicsElement"/> for an imported PNG and its <see cref="SpriteElementJson"/> metadata.</summary>
	public static GraphicsElement FromImage(SpriteElementJson json, Image<Rgba32> image, PaletteMap paletteMap, int index, DitheringMethod? ditheringMethod = null)
	{
		ArgumentNullException.ThrowIfNull(json);
		ArgumentNullException.ThrowIfNull(image);

		var flags = json.Flags ?? GraphicsElementFlags.None;
		var xOffset = json.XOffset;
		var yOffset = json.YOffset;
		var zoomOffset = json.ZoomOffset ?? 0;
		var name = json.Name ?? string.Empty;

		// An imported Bgr24 image stays RGBA; every other imported image is palette-format and its source
		// of truth is an indexed frame. Convert (optionally dithered) so company colours survive.
		if (flags.HasFlag(GraphicsElementFlags.IsBgr24))
		{
			return GraphicsElement.FromRgba(flags, image, xOffset, yOffset, zoomOffset, name, index);
		}

		var dither = ditheringMethod.HasValue && ditheringMethod.Value != DitheringMethod.None ? ditheringMethod.Value : (DitheringMethod?)null;
		var frame = dither.HasValue
			? paletteMap.ConvertRgba32ImageToIndexedImageDithering(image, flags, dither.Value)
			: paletteMap.ConvertRgba32ImageToIndexedImage(image, flags);

		return GraphicsElement.FromIndexed(flags, frame, xOffset, yOffset, zoomOffset, name, index);
	}
}
