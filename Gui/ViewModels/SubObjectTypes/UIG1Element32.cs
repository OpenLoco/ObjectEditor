using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Gui.ViewModels
{
	public record UIG1Element32(
		[ReadOnly(true)] int ImageIndex,
		[ReadOnly(true)] string ImageName,
		[ReadOnly(true)] int16_t Width,
		[ReadOnly(true)] int16_t Height,
		int16_t XOffset,
		int16_t YOffset,
		G1ElementFlags Flags,
		int16_t ZoomOffset)
	{
		public UIG1Element32(int imageIndex, string imageName, G1Element32 g1Element32)
			: this(
				imageIndex,
				imageName,
				g1Element32.Width,
				g1Element32.Height,
				g1Element32.XOffset,
				g1Element32.YOffset,
				g1Element32.Flags,
				g1Element32.ZoomOffset)
		{ }
	}
}
