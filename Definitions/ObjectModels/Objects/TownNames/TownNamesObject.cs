using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.TownNames;

public class TownNamesObject : ILocoStruct
{
	[Length(6, 6)]
	public List<Category> Categories { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
