using Definitions.ObjectModels.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Common;

public interface IHasBuildingComponents
{
	BuildingComponents BuildingComponents { get; set; }
}

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BuildingComponents : ILocoStruct
{
	[Length(1, 63)]
	[CountEqualTo(nameof(BuildingAnimations))]
	public List<uint8_t> BuildingHeights { get; set; } = [];

	[Length(1, 63)]
	[CountEqualTo(nameof(BuildingHeights))]
	public List<BuildingPartAnimation> BuildingAnimations { get; set; } = [];

	[Length(1, 31)]
	public List<List<uint8_t>> BuildingVariations { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (BuildingHeights.Count is < 1 or > 63)
		{
			yield return new ValidationResult($"{nameof(BuildingHeights)} must contain between 1 and 63 entries.", [nameof(BuildingHeights)]);
		}

		if (BuildingAnimations.Count is < 1 or > 63)
		{
			yield return new ValidationResult($"{nameof(BuildingAnimations)} must contain between 1 and 63 entries.", [nameof(BuildingAnimations)]);
		}

		if (BuildingHeights.Count != BuildingAnimations.Count)
		{
			yield return new ValidationResult($"{nameof(BuildingHeights)} and {nameof(BuildingAnimations)} must contain the same number of entries.", [nameof(BuildingHeights), nameof(BuildingAnimations)]);
		}

		if (BuildingVariations.Count is < 1 and <= 31)
		{
			yield return new ValidationResult($"{nameof(BuildingVariations)} must contain between 1 and 31 entries.", [nameof(BuildingVariations)]);
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

		yield break;
	}
}
