using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Water;

public class WaterObject : ILocoStruct
{
	public uint8_t CostIndex { get; set; }
	public uint8_t var_03 { get; set; }
	public int16_t CostFactor { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (CostIndex >= Constants.CurrencyMultiplicationFactorArraySize)
		{
			yield return new ValidationResult($"{nameof(CostIndex)} must be less than {Constants.CurrencyMultiplicationFactorArraySize}", [nameof(CostIndex)]);
		}

		if (CostFactor <= 0)
		{
			yield return new ValidationResult($"{nameof(CostFactor)} must be positive.", [nameof(CostFactor)]);
		}
	}
}
