using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Currency;

public class CurrencyObject : ILocoStruct
{
	public uint8_t Separator { get; set; }
	public uint8_t Factor { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (Separator > 4)
		{
			yield return new ValidationResult("Separator must be between 0 and 4", [nameof(Separator)]);
		}

		if (Factor > 3)
		{
			yield return new ValidationResult("Factor must be between 0 and 3", [nameof(Factor)]);
		}
	}
}
