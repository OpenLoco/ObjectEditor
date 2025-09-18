using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.CliffEdge;

public class CliffEdgeObject : ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
