using Definitions.Database.Base;
using Definitions.ObjectModels.Objects.Wall;

namespace Definitions.DTO.Objects;

public class DtoObjectWall : IDtoSubObject
{
	public uint8_t Height { get; set; }
	public WallObjectFlags1 Flags1 { get; set; }
	public UniqueObjectId Id { get; set; }
}
