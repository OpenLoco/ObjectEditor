using Definitions.Database;
using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public class DtoObjectSteam : IDtoSubObject
{
	public uint8_t NumStationaryTicks { get; set; }
	public uint8_t SpriteWidth { get; set; }
	public uint8_t SpriteHeightNegative { get; set; }
	public uint8_t SpriteHeightPositive { get; set; }
	public SteamObjectFlags Flags { get; set; }
	public uint32_t ImageOffset { get; set; }
	public List<SteamImageAndHeight> FrameInfoType0 { get; set; } = [];
	public List<SteamImageAndHeight> FrameInfoType1 { get; set; } = [];
	public List<ObjectModelHeader> SoundEffects { get; set; } = [];
	public UniqueObjectId Id { get; set; }
}
