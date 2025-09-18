using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.LevelCrossing;

public class LevelCrossingObject : ILocoStruct
{
	public int16_t CostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t AnimationSpeed { get; set; }
	public uint8_t ClosingFrames { get; set; }
	public uint8_t ClosedFrames { get; set; }
	public uint8_t var_0A { get; set; } // something like IdleAnimationFrames or something
	public uint16_t DesignedYear { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (-SellCostFactor > CostFactor)
		{
			yield return new ValidationResult("SellCostFactor must not be greater than CostFactor", [nameof(SellCostFactor), nameof(CostFactor)]);
		}

		if (CostFactor <= 0)
		{
			yield return new ValidationResult("CostFactor must be positive", [nameof(CostFactor)]);
		}

		if (ClosingFrames is not 1 or 2 or 4 or 8 or 16 or 32)
		{
			yield return new ValidationResult("ClosingFrames must be a power of two between 1 and 32 (inclusive)", [nameof(ClosingFrames)]);
		}
	}
}
