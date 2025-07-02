using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectSteam : DbSubObject, IConvertibleToTable<TblObjectSteam, SteamObject>
	{
		public uint8_t NumStationaryTicks { get; set; }
		public uint8_t SpriteWidth { get; set; }
		public uint8_t SpriteHeightNegative { get; set; }
		public uint8_t SpriteHeightPositive { get; set; }
		public SteamObjectFlags Flags { get; set; }

		//public uint32_t var_0A {get; set; }
		//public ICollection<object_id> SoundEffects {get; set; }

		public static TblObjectSteam FromObject(TblObject tbl, SteamObject obj)
			=> new()
			{
				Parent = tbl,
				NumStationaryTicks = obj.NumStationaryTicks,
				SpriteWidth = obj.SpriteWidth,
				SpriteHeightNegative = obj.SpriteHeightNegative,
				SpriteHeightPositive = obj.SpriteHeightPositive,
				Flags = obj.Flags,
			};
	}
}
