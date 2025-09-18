using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Climate;

public class ClimateObject : ILocoStruct
{
	public uint8_t FirstSeason { get; set; }
	public uint8_t SeasonLength1 { get; set; }
	public uint8_t SeasonLength2 { get; set; }
	public uint8_t SeasonLength3 { get; set; }
	public uint8_t SeasonLength4 { get; set; }
	public uint8_t WinterSnowLine { get; set; }
	public uint8_t SummerSnowLine { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (WinterSnowLine > SummerSnowLine)
		{
			yield return new ValidationResult("WinterSnowLine must be less than or equal to SummerSnowLine", [nameof(WinterSnowLine), nameof(SummerSnowLine)]);
		}

		if (FirstSeason >= 4)
		{
			yield return new ValidationResult("FirstSeason must be less than 4", [nameof(FirstSeason)]);
		}
	}
}
