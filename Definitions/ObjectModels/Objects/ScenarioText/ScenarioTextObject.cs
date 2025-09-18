using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.ScenarioText;
public class ScenarioTextObject : ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
