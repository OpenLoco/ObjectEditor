using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.LevelCrossing;

public class LevelCrossingObject : ILocoStruct
{
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t AnimationSpeed { get; set; }
	public uint8_t ClosingFrames { get; set; }
	public uint8_t ClosedFrames { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint8_t var_0A { get; set; } // something like IdleAnimationFrames or something

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (SellCostFactor >= 0)
		{
			yield return new ValidationResult($"{nameof(SellCostFactor)} must be less than 0 {nameof(SellCostFactor)}", [nameof(SellCostFactor)]);
		}

		if (BuildCostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(BuildCostFactor)} must be greater than 0", [nameof(BuildCostFactor)]);
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			yield return new ValidationResult($"-{nameof(SellCostFactor)} must be less than or equal to {nameof(BuildCostFactor)}.", [nameof(SellCostFactor), nameof(BuildCostFactor)]);
		}

		if (ClosingFrames is not 1 or 2 or 4 or 8 or 16 or 32)
		{
			yield return new ValidationResult("ClosingFrames must be a power of two between 1 and 32 (inclusive)", [nameof(ClosingFrames)]);
		}
	}
}
