using Definitions.ObjectModels.Objects.Steam;
using System.Text.Json;

namespace Definitions.Database;

public class TblObjectSteam : DbSubObject, IConvertibleToTable<TblObjectSteam, SteamObject>
{
	public uint8_t NumStationaryTicks { get; set; }
	public uint8_t SpriteWidth { get; set; }
	public uint8_t SpriteHeightNegative { get; set; }
	public uint8_t SpriteHeightPositive { get; set; }
	public SteamObjectFlags Flags { get; set; }
	public uint32_t var_0A { get; set; }
	public string FrameInfoType0 { get; set; } = "[]";
	public string FrameInfoType1 { get; set; } = "[]";
	public string SoundEffects { get; set; } = "[]";

	public static TblObjectSteam FromObject(TblObject tbl, SteamObject obj)
		=> new()
		{
			Parent = tbl,
			NumStationaryTicks = obj.NumStationaryTicks,
			SpriteWidth = obj.SpriteWidth,
			SpriteHeightNegative = obj.SpriteHeightNegative,
			SpriteHeightPositive = obj.SpriteHeightPositive,
			Flags = obj.Flags,
			var_0A = obj.var_0A,
			FrameInfoType0 = JsonSerializer.Serialize(obj.FrameInfoType0),
			FrameInfoType1 = JsonSerializer.Serialize(obj.FrameInfoType1),
			SoundEffects = JsonSerializer.Serialize(obj.SoundEffects),
		};
}
