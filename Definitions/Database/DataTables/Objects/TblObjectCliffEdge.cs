using Definitions.ObjectModels.Objects.CliffEdge;

namespace Definitions.Database;

public class TblObjectCliffEdge : DbSubObject, IConvertibleToTable<TblObjectCliffEdge, CliffEdgeObject>
{
	public static TblObjectCliffEdge FromObject(TblObject tbl, CliffEdgeObject obj)
		=> new()
		{
			Parent = tbl,
		};
}
