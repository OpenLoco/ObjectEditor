using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectWall : DbSubObject, IConvertibleToTable<TblObjectWall, WallObject>
	{
		public uint8_t Height { get; set; }
		public WallObjectFlags1 Flags1 { get; set; }

		public static TblObjectWall FromObject(TblObject tbl, WallObject obj)
			=> new()
			{
				Parent = tbl,
				Height = obj.Height,
				Flags1 = obj.Flags1,
			};
	}
}
