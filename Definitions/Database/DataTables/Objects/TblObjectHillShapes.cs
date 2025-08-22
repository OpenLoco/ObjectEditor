using Definitions.ObjectModels.Objects.HillShape;

namespace Definitions.Database;

public class TblObjectHillShapes : DbSubObject, IConvertibleToTable<TblObjectHillShapes, HillShapesObject>
{
	public uint8_t HillHeightMapCount { get; set; }
	public uint8_t MountainHeightMapCount { get; set; }
	public bool IsHeightMap { get; set; }

	public static TblObjectHillShapes FromObject(TblObject tbl, HillShapesObject obj)
		=> new()
		{
			Parent = tbl,
			HillHeightMapCount = obj.HillHeightMapCount,
			MountainHeightMapCount = obj.MountainHeightMapCount,
			IsHeightMap = obj.IsHeightMap,
		};
}
