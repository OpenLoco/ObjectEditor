using Dat.Types;
using Definitions.ObjectModels.Graphics;

namespace Dat.Converters;

public static class GraphicsElementConverter
{
	/// <summary>Encodes an in-memory <see cref="GraphicsElement"/> into the DAT serialisation DTO,
	/// deriving the raw palette/row bytes from the image.</summary>
	public static DatG1Element32 ToDatG1Element32(this GraphicsElement graphicsImage, PaletteMap paletteMap)
		=> new(
			0U, // Offset is not used in the DatG1Element32, it is set later when writing to the file
			(int16_t)graphicsImage.Width,
			(int16_t)graphicsImage.Height,
			graphicsImage.XOffset,
			graphicsImage.YOffset,
			(DatG1ElementFlags)graphicsImage.Flags,
			graphicsImage.ZoomOffset
		)
		{
			ImageData = graphicsImage.ToG1Data(paletteMap),
		};

	/// <summary>Decodes a raw DAT <see cref="DatG1Element32"/> into the in-memory <see cref="GraphicsElement"/>
	/// model. Prefers an indexed frame (preserving reserved/company indices) and falls back to a decoded
	/// RGBA image for Bgr24 data. The palette is taken from the global palette service.</summary>
	public static GraphicsElement Convert(this DatG1Element32 g1Element)
	{
		var flags = (GraphicsElementFlags)g1Element.Flags;
		var palette = PaletteMapLoader.Current;

		// Prefer an indexed frame (preserving reserved/company indices verbatim); fall back to a decoded
		// RGBA image for Bgr24 data. The palette is taken from the global palette service.
		if (palette.TryConvertImageDataToIndexedImage(g1Element.Width, g1Element.Height, flags, g1Element.ImageData, out var frame))
		{
			return GraphicsElement.FromIndexed(flags, frame, g1Element.XOffset, g1Element.YOffset, g1Element.ZoomOffset);
		}

		return GraphicsElement.FromRgba(flags, palette.ConvertImageDataToRgba32Bitmap(g1Element.Width, g1Element.Height, flags, g1Element.ImageData), g1Element.XOffset, g1Element.YOffset, g1Element.ZoomOffset);
	}
}
