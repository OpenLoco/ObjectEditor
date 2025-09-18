using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Tunnel;

public class TunnelObject : ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
