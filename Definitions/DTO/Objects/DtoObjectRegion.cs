using Definitions.Database;
using Definitions.ObjectModels.Objects.Region;
using Definitions.ObjectModels.Types;

namespace Definitions.DTO;

public class DtoObjectRegion : IDtoSubObject
{
	public DrivingSide VehicleDrivingSide { get; set; }
	public List<ObjectModelHeader> CargoInfluenceObjects { get; set; } = [];
	public List<ObjectModelHeader> DependentObjects { get; set; } = [];
	public List<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; set; } = [];
	public UniqueObjectId Id { get; set; }
}
