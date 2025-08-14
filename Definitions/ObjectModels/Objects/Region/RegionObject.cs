using Definitions.ObjectModels.Types;

namespace Definitions.ObjectModels.Objects.Region;

public class RegionObject : ILocoStruct
{
	public List<ObjectModelHeader> CargoInfluenceObjects { get; set; }
	public List<ObjectModelHeader> DependentObjects { get; set; }
	public List<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; set; }

	public bool Validate()
		=> true;
}
