using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels;

public interface ILocoStruct
{
	IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
}
