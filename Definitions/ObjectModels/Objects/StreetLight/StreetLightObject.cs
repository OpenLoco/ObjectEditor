using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Streetlight;

public class StreetLightObject : ILocoStruct
{
	[Length(3, 3)]
	public List<uint16_t> DesignedYears { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
