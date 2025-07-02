using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectHillShapes : DbSubObject, IConvertibleToTable<TblObjectHillShapes, HillShapesObject>
	{
		public uint8_t HillHeightMapCount { get; set; }
		public uint8_t MountainHeightMapCount { get; set; }
		public HillShapeFlags Flags { get; set; }

		public static TblObjectHillShapes FromObject(TblObject tbl, HillShapesObject obj)
			=> new()
			{
				Parent = tbl,
				HillHeightMapCount = obj.HillHeightMapCount,
				MountainHeightMapCount = obj.MountainHeightMapCount,
				Flags = obj.Flags,
			};
	}
}
