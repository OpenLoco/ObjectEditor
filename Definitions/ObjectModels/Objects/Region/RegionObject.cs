using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Region;

public class RegionObject : ILocoStruct
{
	public DrivingSide VehicleDrivingSide { get; set; }
	public List<ObjectModelHeader> CargoInfluenceObjects { get; set; } = [];
	public List<ObjectModelHeader> DependentObjects { get; set; } = [];
	public List<CargoInfluenceTownFilterType> CargoInfluenceTownFilter { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
