using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace AvaGui.ViewModels
{
	public record UIG1Element32(
		[Category("Image")] int ImageIndex,
		[Category("Image")] string ImageName,
		[Category("G1Element32")] uint32_t Offset,
		[Category("G1Element32")] int16_t Width,
		[Category("G1Element32")] int16_t Height,
		[Category("G1Element32")] int16_t XOffset,
		[Category("G1Element32")] int16_t YOffset,
		[Category("G1Element32")] G1ElementFlags Flags,
		[Category("G1Element32")] int16_t ZoomOffset
	)
	{
		public UIG1Element32(int imageIndex, string imageName, G1Element32 g1Element)
			: this(
				imageIndex,
				imageName,
				g1Element.Offset,
				g1Element.Width,
				g1Element.Height,
				g1Element.XOffset,
				g1Element.YOffset,
				g1Element.Flags,
				g1Element.ZoomOffset)
		{ }
	}
}
