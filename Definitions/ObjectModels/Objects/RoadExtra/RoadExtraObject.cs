using Definitions.ObjectModels.Objects.Road;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.RoadExtra;

public class RoadExtraObject : ILocoStruct
{
	public RoadTraitFlags RoadPieces { get; set; } = RoadTraitFlags.None;
	public uint8_t PaintStyle { get; set; }
	public uint8_t CostIndex { get; set; }
	public int16_t BuildCostFactor { get; set; }
	public int16_t SellCostFactor { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (PaintStyle >= 2)
		{
			yield return new ValidationResult("PaintStyle must be 0 or 1", [nameof(PaintStyle)]);
		}

		if (CostIndex >= Constants.CurrencyMultiplicationFactorArraySize)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be less than {Constants.CurrencyMultiplicationFactorArraySize}", [nameof(CostIndex)]);
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			yield return new ValidationResult("SellCostFactor must be greater than or equal to -BuildCostFactor", [nameof(SellCostFactor), nameof(BuildCostFactor)]);

		}

		if (BuildCostFactor <= 0)
		{
			yield return new ValidationResult("BuildCostFactor must be greater than 0", [nameof(BuildCostFactor)]);
		}
	}
}
