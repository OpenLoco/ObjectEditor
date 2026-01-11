using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.ScenarioText;

public class ScenarioTextObject : ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
