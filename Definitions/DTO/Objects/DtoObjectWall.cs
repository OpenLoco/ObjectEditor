using Definitions.Database;
using Definitions.ObjectModels.Objects.Wall;

namespace Definitions.DTO;

public class DtoObjectWall : IDtoSubObject
{
	public uint8_t Height { get; set; }
	public WallObjectFlags1 Flags1 { get; set; }
	public uint8_t ToolId { get; set; }
	public WallObjectFlags2 Flags2 { get; set; } = WallObjectFlags2.None;
	public UniqueObjectId Id { get; set; }
}
