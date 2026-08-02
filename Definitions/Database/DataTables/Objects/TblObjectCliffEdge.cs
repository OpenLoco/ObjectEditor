using Definitions.Database.Base;
using Definitions.ObjectModels.Objects.CliffEdge;

namespace Definitions.Database.DataTables.Objects;

public class TblObjectCliffEdge : DbSubObject, IConvertibleToTable<TblObjectCliffEdge, CliffEdgeObject>
{
	// no data

	public static TblObjectCliffEdge FromObject(TblObject tbl, CliffEdgeObject obj)
		=> new()
		{
			Parent = tbl,
		};
}
