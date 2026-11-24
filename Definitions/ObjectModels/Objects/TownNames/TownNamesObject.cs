using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.TownNames;

public class TownNamesObject : ILocoStruct
{
	[Length(6, 6)]
	public List<MorphemeCategory> MorphemeCategories { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (MorphemeCategories?.Count != 6)
		{
			yield return new ValidationResult($"DAT requirement: {nameof(MorphemeCategories)} must have exactly 6 entries.", [nameof(MorphemeCategories)]);
		}
	}
}
