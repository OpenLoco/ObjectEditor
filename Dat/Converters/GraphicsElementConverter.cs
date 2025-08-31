using Dat.Types;
using Definitions.ObjectModels.Types;

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
