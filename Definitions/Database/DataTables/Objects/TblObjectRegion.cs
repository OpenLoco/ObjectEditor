using OpenLoco.Dat.Objects;

namespace OpenLoco.Definitions.Database
{
	public class TblObjectRegion : DbSubObject, IConvertibleToTable<TblObjectRegion, RegionObject>
	{
		//public uint8_t NumCargoInfluenceObjects { get; set; }
		//public object_id[] _CargoInfluenceObjectIds { get; set; }
		//public CargoInfluenceTownFilterType[] CargoInfluenceTownFilter { get; set; }

		public static TblObjectRegion FromObject(TblObject tbl, RegionObject obj)
			=> new()
			{
				Parent = tbl,
			};
	}
}
