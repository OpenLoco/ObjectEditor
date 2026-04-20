using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects;

public abstract class LocoObjectDefinitionBase : ILocoObjectNOF, ILocoValidation
{
	public required string DisplayName { get; set; }
	public required string InternalName { get; set; }

	public ObjectType ObjectType { get; set; }
	public ObjectSource ObjectSource { get; set; }

	public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
}
