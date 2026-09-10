using Dat.Types;
using Definitions.ObjectModels.Graphics;

namespace Dat.Converters;

public static class GraphicsElementConverter
{
	public static DatG1Element32 Convert(this GraphicsElement graphicsElement)
		=> new(
			0U, // Offset is not used in the DatG1Element32, it is set later when writing to the file
			graphicsElement.Width,
			graphicsElement.Height,
			graphicsElement.XOffset,
			graphicsElement.YOffset,
			(DatG1ElementFlags)graphicsElement.Flags,
			graphicsElement.ZoomOffset
		)
		{
			ImageData = graphicsElement.ImageData,
		};

	/// <summary>Encodes an in-memory <see cref="GraphicsImage"/> into the DAT serialisation DTO,
	/// deriving the raw palette/row bytes from the image.</summary>
	public static DatG1Element32 ToDatG1Element32(this GraphicsImage graphicsImage, PaletteMap paletteMap)
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

	public static GraphicsElement Convert(this DatG1Element32 graphicsElement)
		=> new()
		{
			Width = graphicsElement.Width,
			Height = graphicsElement.Height,
			XOffset = graphicsElement.XOffset,
			YOffset = graphicsElement.YOffset,
			Flags = (GraphicsElementFlags)graphicsElement.Flags,
			ZoomOffset = graphicsElement.ZoomOffset,
			ImageData = graphicsElement.ImageData,
		};
}
