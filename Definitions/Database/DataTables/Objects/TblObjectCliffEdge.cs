using Dat.Objects;

namespace Definitions.Database
{
	public class TblObjectCliffEdge : DbSubObject, IConvertibleToTable<TblObjectCliffEdge, CliffEdgeObject>
	{
		// no data

		public static TblObjectCliffEdge FromObject(TblObject tbl, CliffEdgeObject obj)
			=> new()
			{
				Parent = tbl,
			};
	}
}
