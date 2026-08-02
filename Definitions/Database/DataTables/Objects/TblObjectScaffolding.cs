using Definitions.Database.Base;
using Definitions.ObjectModels.Objects.Scaffolding;

namespace Definitions.Database.DataTables.Objects;

public class TblObjectScaffolding : DbSubObject, IConvertibleToTable<TblObjectScaffolding, ScaffoldingObject>
{
	//public ICollection<uint16_t> SegmentHeights { get; set; }
	//public ICollection<uint16_t> RoofHeights { get; set; }

	public static TblObjectScaffolding FromObject(TblObject tbl, ScaffoldingObject obj)
		=> new()
		{
			Parent = tbl,
		};
}
