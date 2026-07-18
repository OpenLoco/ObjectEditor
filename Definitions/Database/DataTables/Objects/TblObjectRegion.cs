using Definitions.ObjectModels.Objects.Region;
using System.Text.Json;

namespace Definitions.Database;

public class TblObjectRegion : DbSubObject, IConvertibleToTable<TblObjectRegion, RegionObject>
{
	public string VehicleDrivingSide { get; set; } = string.Empty;
	public string CargoInfluenceTownFilter { get; set; } = "[]";
	public string CargoInfluenceObjects { get; set; } = "[]";
	public string DependentObjects { get; set; } = "[]";

	public static TblObjectRegion FromObject(TblObject tbl, RegionObject obj)
		=> new()
		{
			Parent = tbl,
			VehicleDrivingSide = obj.VehicleDrivingSide.ToString(),
			CargoInfluenceTownFilter = JsonSerializer.Serialize(obj.CargoInfluenceTownFilter),
			CargoInfluenceObjects = JsonSerializer.Serialize(obj.CargoInfluenceObjects),
			DependentObjects = JsonSerializer.Serialize(obj.DependentObjects),
		};
}

