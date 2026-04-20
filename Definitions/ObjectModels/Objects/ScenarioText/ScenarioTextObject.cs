using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.ScenarioText;

public class ScenarioTextObject : ILocoValidation
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
