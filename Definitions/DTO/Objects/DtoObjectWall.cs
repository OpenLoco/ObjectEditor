using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.DTO
{
	public class DtoObjectWall : IHasId
	{
		public uint8_t Height { get; set; }
		public WallObjectFlags1 Flags1 { get; set; }
		public UniqueObjectId Id { get; set; }
	}
}
