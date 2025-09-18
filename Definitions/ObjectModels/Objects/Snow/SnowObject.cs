using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Snow;

public class SnowObject : ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
