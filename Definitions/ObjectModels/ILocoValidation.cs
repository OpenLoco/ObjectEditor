using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels;

public interface ILocoValidation
{
	IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
}
