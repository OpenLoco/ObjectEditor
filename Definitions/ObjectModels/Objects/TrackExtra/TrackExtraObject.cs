using Definitions.ObjectModels.Objects.Track;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.TrackExtra;

public class TrackExtraObject : ILocoStruct
{
	public TrackTraitFlags TrackPieces { get; set; }
	public uint8_t PaintStyle { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (PaintStyle >= 2)
		{
			yield return new ValidationResult($"{nameof(PaintStyle)} must be either 0 (normal) or 1 (fancy).", [nameof(PaintStyle)]);
		}

		// This check missing from vanilla
		if (CostIndex > 32)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be between 0 and 32 inclusive.", [nameof(CostIndex)]);
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			yield return new ValidationResult($"The negative of {nameof(SellCostFactor)} must be less than or equal to {nameof(BuildCostFactor)}.", [nameof(SellCostFactor), nameof(BuildCostFactor)]);
		}

		if (BuildCostFactor > 0)
		{
			yield return new ValidationResult($"{nameof(BuildCostFactor)} must be greater than 0.", [nameof(BuildCostFactor)]);
		}
	}
}
