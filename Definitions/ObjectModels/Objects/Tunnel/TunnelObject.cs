using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Tunnel;

public class TunnelObject : ILocoValidation
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
