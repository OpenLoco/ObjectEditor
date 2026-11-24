using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Types;

namespace Definitions.Database;

public class TblObjectRegion : DbSubObject, IConvertibleToTable<TblObjectRegion, RegionObject>
{
	public DrivingSide VehicleDrivingSide { get; set; }
	public List<ObjectModelHeader> CargoInfluenceObjects { get; set; } = [];
	public List<ObjectModelHeader> DependentObjects { get; set; } = [];
	public List<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; set; } = [];

	public static TblObjectRegion FromObject(TblObject tbl, RegionObject obj)
		=> new()
		{
			Parent = tbl,
			VehicleDrivingSide = obj.VehicleDrivingSide,
			CargoInfluenceObjects = obj.CargoInfluenceObjects,
			DependentObjects = obj.DependentObjects,
			CargoInfluenceTownFilter = obj.CargoInfluenceTownFilter,
		};
}
