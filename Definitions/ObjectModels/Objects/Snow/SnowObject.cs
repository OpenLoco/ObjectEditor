using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Snow;

public class SnowObject : ILocoValidation
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
