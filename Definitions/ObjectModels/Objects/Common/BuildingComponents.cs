using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Common;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BuildingComponents : ILocoStruct
{
	[Length(1, BuildingObject.Constants.MaxAnimationsCount)]
	[CountEqualTo(nameof(BuildingAnimations))]
	public List<uint8_t> BuildingHeights { get; set; } = [];

	[Length(1, BuildingObject.Constants.MaxHeightsCount)]
	[CountEqualTo(nameof(BuildingHeights))]
	public List<BuildingPartAnimation> BuildingAnimations { get; set; } = [];

	[Length(1, BuildingObject.Constants.MaxVariationsCount)]
	public List<List<uint8_t>> BuildingVariations { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		// heights and animations
		{
			if (BuildingHeights.Count is < 1 or > BuildingObject.Constants.MaxHeightsCount)
			{
				yield return new ValidationResult($"{nameof(BuildingHeights)} must contain between 1 and {BuildingObject.Constants.MaxHeightsCount} entries.", [nameof(BuildingHeights)]);
			}

			if (BuildingAnimations.Count is < 1 or > BuildingObject.Constants.MaxAnimationsCount)
			{
				yield return new ValidationResult($"{nameof(BuildingAnimations)} must contain between 1 and {BuildingObject.Constants.MaxAnimationsCount} entries.", [nameof(BuildingAnimations)]);
			}

			if (BuildingHeights.Count != BuildingAnimations.Count)
			{
				yield return new ValidationResult($"{nameof(BuildingHeights)} and {nameof(BuildingAnimations)} must contain the same number of entries.", [nameof(BuildingHeights), nameof(BuildingAnimations)]);
			}
		}

		// variations
		{
			if (BuildingVariations.Count is < 1 or > BuildingObject.Constants.MaxVariationsCount)
			{
				yield return new ValidationResult($"{nameof(BuildingVariations)} must contain between 1 and {BuildingObject.Constants.MaxVariationsCount} entries.", [nameof(BuildingVariations)]);
			}

			foreach (var bv in BuildingVariations)
			{
				foreach (var bvl in bv)
				{
					if (bvl >= BuildingHeights.Count)
					{
						yield return new ValidationResult($"A building variation layer index ({bvl}) is out of range. It must be less than the number of building heights ({BuildingHeights.Count}).", [nameof(BuildingVariations)]);
					}
				}
			}
		}

		yield break;
	}
}
