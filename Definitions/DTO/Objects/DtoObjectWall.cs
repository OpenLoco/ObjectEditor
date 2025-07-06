using OpenLoco.Dat.Objects;
using OpenLoco.Definitions.Database;

namespace OpenLoco.Definitions.DTO
{
	public class DtoObjectWall : IDtoSubObject
	{
		public uint8_t Height { get; set; }
		public WallObjectFlags1 Flags1 { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
