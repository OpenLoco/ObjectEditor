using Definitions.ObjectModels.Objects.Scaffolding;

namespace Definitions.Database;

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
