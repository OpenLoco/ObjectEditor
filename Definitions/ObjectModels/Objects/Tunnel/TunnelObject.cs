using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Tunnel;

public class TunnelObject : ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
