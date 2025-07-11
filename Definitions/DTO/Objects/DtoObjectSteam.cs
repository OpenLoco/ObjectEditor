using Dat.Objects;
using Definitions.Database;

namespace Definitions.DTO
{
	public class DtoObjectSteam : IDtoSubObject
	{
		public uint8_t NumStationaryTicks { get; set; }
		public uint8_t SpriteWidth { get; set; }
		public uint8_t SpriteHeightNegative { get; set; }
		public uint8_t SpriteHeightPositive { get; set; }
		public SteamObjectFlags Flags { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
