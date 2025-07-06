using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
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
}
