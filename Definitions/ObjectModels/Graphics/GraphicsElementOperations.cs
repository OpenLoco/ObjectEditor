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

		var rgba = image.ToRgba(paletteMap);
		var cropRegion = FindCropRegion(rgba);

		if (cropRegion.Width <= 0 || cropRegion.Height <= 0)
		{
			// fully transparent image - collapse to a 1x1 transparent pixel (same fallback the GUI uses)
			image.ReplaceDecoded(rgba.Clone(i => i.Crop(new Rectangle(0, 0, 1, 1))), null);
			image.XOffset = 0;
			image.YOffset = 0;
			return;
		}

		image.ReplaceDecoded(rgba.Clone(i => i.Crop(cropRegion)), null);
		image.XOffset = (short)(image.XOffset + cropRegion.Left);
		image.YOffset = (short)(image.YOffset + cropRegion.Top);
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

		var flags = json.Flags ?? GraphicsElementFlags.None;
		var graphicsImage = GraphicsElement.FromRgba(flags, image, json.XOffset, json.YOffset, json.ZoomOffset ?? 0, json.Name ?? string.Empty, index);

		// Bake the palette (optionally dithered) bytes in so the import round-trips and serialises correctly.
		graphicsImage.ImageData = graphicsImage.ToG1Data(paletteMap, ditheringMethod);
		// Materialise the indexed (dithered) frame so previews and save both reflect the chosen dithering.
		_ = graphicsImage.ToIndexed(paletteMap, ditheringMethod);

		return graphicsImage;
	}
}
