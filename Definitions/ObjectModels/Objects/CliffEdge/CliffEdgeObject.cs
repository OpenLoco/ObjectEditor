using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.CliffEdge;

public class CliffEdgeObject : ILocoValidation
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
