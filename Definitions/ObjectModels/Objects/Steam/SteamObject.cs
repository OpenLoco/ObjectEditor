using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.Steam;

public class SteamObject : ILocoStruct
{
	public uint8_t NumStationaryTicks { get; set; }
	public uint8_t SpriteWidth { get; set; }
	public uint8_t SpriteHeightNegative { get; set; }
	public uint8_t SpriteHeightPositive { get; set; }
	public SteamObjectFlags Flags { get; set; }
	public uint32_t var_0A { get; set; }

	public List<ImageAndHeight> FrameInfoType0 { get; set; } = [];
	public List<ImageAndHeight> FrameInfoType1 { get; set; } = [];

	public List<ObjectModelHeader> SoundEffects { get; set; } = [];

	public bool Validate()
		=> true;
}
