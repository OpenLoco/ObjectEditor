using Definitions.ObjectModels.Objects.Wall;

namespace Definitions.Database;

public class TblObjectWall : DbSubObject, IConvertibleToTable<TblObjectWall, WallObject>
{
	public uint8_t ToolId { get; set; } // unused in loco???
	public WallObjectFlags1 Flags1 { get; set; } = WallObjectFlags1.None;
	public uint8_t Height { get; set; }
	public WallObjectFlags2 Flags2 { get; set; } = WallObjectFlags2.None; // unused in loco???

	public static TblObjectWall FromObject(TblObject tbl, WallObject obj)
		=> new()
		{
			Parent = tbl,
			ToolId = obj.ToolId,
			Flags1 = obj.Flags1,
			Height = obj.Height,
			Flags2 = obj.Flags2,
		};
}
