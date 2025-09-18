using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Tree;

public class TreeObject : ILocoStruct
{
	public uint8_t Clearance { get; set; }
	public uint8_t Height { get; set; }
	public uint8_t var_04 { get; set; }
	public uint8_t var_05 { get; set; }
	public uint8_t NumRotations { get; set; }
	public uint8_t NumGrowthStages { get; set; }
	public TreeObjectFlags Flags { get; set; }
	public uint16_t ShadowImageOffset { get; set; }
	public uint8_t SeasonState { get; set; }
	public uint8_t Season { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t ClearCostFactor { get; set; }
	public uint32_t Colours { get; set; }
	public int16_t Rating { get; set; }
	public int16_t DemolishRatingReduction { get; set; }
	public TreeFlagsUnk var_3C { get; set; } // something with images

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (CostIndex >= Constants.CurrencyMultiplicationFactorArraySize)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be less than {Constants.CurrencyMultiplicationFactorArraySize}", [nameof(CostIndex)]);
		}

		// 230/256 = ~90%
		if (-ClearCostFactor > BuildCostFactor * 230 / 256)
		{
			yield return new ValidationResult($"The negative of {nameof(ClearCostFactor)} must be less than or equal to ~90% of {nameof(BuildCostFactor)}.", [nameof(ClearCostFactor), nameof(BuildCostFactor)]);
		}

		if (NumRotations is not 1 or 2 or 4)
		{
			yield return new ValidationResult($"{nameof(NumRotations)} must be either 1, 2, or 4.", [nameof(NumRotations)]);
		}

		if (NumGrowthStages is < 1 or > 8)
		{
			yield return new ValidationResult($"{nameof(NumGrowthStages)} must be between 1 and 8 inclusive.", [nameof(NumGrowthStages)]);
		}

		if (Height < Clearance)
		{
			yield return new ValidationResult($"{nameof(Height)} must be greater than or equal to {nameof(Clearance)}.", [nameof(Height), nameof(Clearance)]);
		}

		if (var_05 < var_04)
		{
			yield return new ValidationResult($"{nameof(var_05)} must be greater than or equal to {nameof(var_04)}.", [nameof(var_05), nameof(var_04)]);
		}
	}
}
