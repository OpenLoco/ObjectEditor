using Definitions.ObjectModels.Graphics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Definitions.ObjectModels.Graphics;

public static class GraphicsElementOperations
{
	public static void SetImage(this GraphicsElement element, Image<Rgba32> image, PaletteMap paletteMap)
	{
		ArgumentNullException.ThrowIfNull(element);
		ArgumentNullException.ThrowIfNull(image);
		ArgumentNullException.ThrowIfNull(paletteMap);

		if (!ReferenceEquals(element.Image, image))
		{
			element.Image?.Dispose();
			element.Image = image;
		}

		element.Width = (short)image.Width;
		element.Height = (short)image.Height;
		element.ImageData = paletteMap.ConvertRgba32ImageToG1Data(image, element.Flags);
	}

	public static void ReplaceImage(this GraphicsElement element, string pngFileName, PaletteMap paletteMap)
		=> element.SetImage(Image.Load<Rgba32>(pngFileName), paletteMap);

	public static void SyncImageData(this GraphicsElement element, PaletteMap paletteMap)
	{
		ArgumentNullException.ThrowIfNull(element);

		if (element.Image == null)
		{
			return;
		}

		element.SetImage(element.Image, paletteMap);
	}

	public static void Decode(this GraphicsElement element, PaletteMap paletteMap, ColourSwatch primary = ColourSwatch.PrimaryRemap, ColourSwatch secondary = ColourSwatch.SecondaryRemap)
	{
		ArgumentNullException.ThrowIfNull(element);
		ArgumentNullException.ThrowIfNull(paletteMap);

		element.Image = paletteMap.TryConvertG1ToRgba32Bitmap(element, primary, secondary, out var image)
			? image
			: ImageTableHelpers.ErrorImage;
	}

	public static void Crop(this GraphicsElement element, PaletteMap paletteMap)
	{
		ArgumentNullException.ThrowIfNull(element);

		var image = element.Image;
		if (image == null)
		{
			return;
		}

		var cropRegion = FindCropRegion(image);

		if (cropRegion.Width <= 0 || cropRegion.Height <= 0)
		{
			element.SetImage(image.Clone(i => i.Crop(new Rectangle(0, 0, 1, 1))), paletteMap);
			element.XOffset = 0;
			element.YOffset = 0;
		}
		else
		{
			element.SetImage(image.Clone(i => i.Crop(cropRegion)), paletteMap);
			element.XOffset += (short)cropRegion.Left;
			element.YOffset += (short)cropRegion.Top;
		}
	}

	public static void ZeroOffsets(this GraphicsElement element)
	{
		ArgumentNullException.ThrowIfNull(element);

		element.XOffset = 0;
		element.YOffset = 0;
	}

	public static void CenterOffsets(this GraphicsElement element)
	{
		ArgumentNullException.ThrowIfNull(element);

		element.XOffset = (short)(-element.Width / 2);
		element.YOffset = (short)(-element.Height / 2);
	}

	public static void TranslateOffsets(this GraphicsElement element, short deltaX, short deltaY)
	{
		ArgumentNullException.ThrowIfNull(element);

		element.XOffset += deltaX;
		element.YOffset += deltaY;
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

	public static GraphicsElement FromImage(GraphicsElementJson json, Image<Rgba32> image, PaletteMap paletteMap, int index)
	{
		ArgumentNullException.ThrowIfNull(json);

		var flags = json.Flags ?? GraphicsElementFlags.None;
		var element = new GraphicsElement()
		{
			Width = (int16_t)image.Width,
			Height = (int16_t)image.Height,
			XOffset = json.XOffset,
			YOffset = json.YOffset,
			Flags = flags,
			ZoomOffset = json.ZoomOffset ?? 0,
			ImageData = paletteMap.ConvertRgba32ImageToG1Data(image, flags),
			Name = json.Name ?? string.Empty,
			Image = image,
			ImageTableIndex = index,
		};

		element.Decode(paletteMap);

		return element;
	}
}
