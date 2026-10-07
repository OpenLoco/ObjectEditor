using Definitions.ObjectModels.Objects.Steam;
using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public class TblObjectSteam : DbSubObject, IConvertibleToTable<TblObjectSteam, SteamObject>
{
	public uint8_t NumStationaryTicks { get; set; }
	public uint8_t SpriteWidth { get; set; }
	public uint8_t SpriteHeightNegative { get; set; }
	public uint8_t SpriteHeightPositive { get; set; }
	public SteamObjectFlags Flags { get; set; }
	public uint32_t ImageOffset { get; set; } // offset added to BaseImageId when drawing exhaust sprites
	public List<SteamImageAndHeight> FrameInfoType0 { get; set; } = [];
	public List<SteamImageAndHeight> FrameInfoType1 { get; set; } = [];
	public List<ObjectModelHeader> SoundEffects { get; set; } = [];

	public static TblObjectSteam FromObject(TblObject tbl, SteamObject obj)
		=> new()
		{
			Parent = tbl,
			NumStationaryTicks = obj.NumStationaryTicks,
			SpriteWidth = obj.SpriteWidth,
			SpriteHeightNegative = obj.SpriteHeightNegative,
			SpriteHeightPositive = obj.SpriteHeightPositive,
			Flags = obj.Flags,
			ImageOffset = obj.ImageOffset,
			FrameInfoType0 = obj.FrameInfoType0,
			FrameInfoType1 = obj.FrameInfoType1,
			SoundEffects = obj.SoundEffects,
		};
}
