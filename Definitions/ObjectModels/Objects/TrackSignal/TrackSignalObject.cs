using Definitions.ObjectModels.Types;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.TrackSignal;

public class TrackSignalObject : ILocoStruct
{
	public TrackSignalObjectFlags Flags { get; set; }
	public uint8_t AnimationSpeed { get; set; }
	public uint8_t NumFrames { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }
	public uint8_t CostIndex { get; set; }
	public uint8_t var_0B { get; set; }
	public uint16_t DesignedYear { get; set; }
	public uint16_t ObsoleteYear { get; set; }
	public List<ObjectModelHeader> CompatibleTrackObjects { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (AnimationSpeed is not 0 and not 1 and not 3 and not 7 and not 15)
		{
			// animationSpeed must be 1 less than a power of 2 (its a mask)
			yield return new ValidationResult($"{nameof(AnimationSpeed)} must be 0, 1, 3, 7, or 15.", [nameof(AnimationSpeed)]);
		}

		if (NumFrames is not 4 or 7 or 10)
		{
			yield return new ValidationResult($"{nameof(NumFrames)} must be 4, 7, or 10.", [nameof(NumFrames)]);
		}

		if (CostIndex >= Constants.CurrencyMultiplicationFactorArraySize)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be less than {Constants.CurrencyMultiplicationFactorArraySize}", [nameof(CostIndex)]);
		}

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

		if (CompatibleTrackObjects.Count > 7)
		{
			yield return new ValidationResult($"{nameof(CompatibleTrackObjects)} must have at most 7 entries.", [nameof(CompatibleTrackObjects)]);
		}
	}
}
