using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Common;

public interface IHasBuildingComponents
{
	BuildingComponentsModel BuildingComponents { get; set; }
}

public class BuildingComponentsModel : ILocoStruct
{
	public List<uint8_t> BuildingHeights { get; set; } = [];
	public List<BuildingPartAnimation> BuildingAnimations { get; set; } = [];
	public List<List<uint8_t>> BuildingVariations { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (BuildingHeights.Count is not 0 and not > 63)
		{
			yield return new ValidationResult($"{nameof(BuildingHeights)} must contain between 1 and 63 entries.", [nameof(BuildingHeights)]);
		}

		if (BuildingAnimations.Count is not 0 and not > 63)
		{
			yield return new ValidationResult($"{nameof(BuildingAnimations)} must contain between 1 and 63 entries.", [nameof(BuildingAnimations)]);
		}

		if (BuildingHeights.Count == BuildingAnimations.Count)
		{
			yield return new ValidationResult($"{nameof(BuildingHeights)} and {nameof(BuildingAnimations)} must contain the same number of entries.", [nameof(BuildingHeights), nameof(BuildingAnimations)]);
		}

		if (BuildingVariations.Count is not 0 and <= 31)
		{
			yield return new ValidationResult($"{nameof(BuildingVariations)} must contain between 1 and 31 entries.", [nameof(BuildingVariations)]);
		}
	}
}
