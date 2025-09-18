using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Snow;

public class SnowObject : ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
