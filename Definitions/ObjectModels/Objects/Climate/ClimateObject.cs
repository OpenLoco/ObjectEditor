using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Climate;

public enum Season
{
	Autumn = 0,
	Winter = 1,
	Spring = 2,
	Summer = 3,
}

public class ClimateObject : ILocoStruct
{
	public Season FirstSeason { get; set; }
	public uint8_t SeasonLength1 { get; set; }
	public uint8_t SeasonLength2 { get; set; }
	public uint8_t SeasonLength3 { get; set; }
	public uint8_t SeasonLength4 { get; set; }
	public uint8_t WinterSnowLine { get; set; }
	public uint8_t SummerSnowLine { get; set; }
	public uint8_t pad_09 { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (WinterSnowLine > SummerSnowLine)
		{
			yield return new ValidationResult("WinterSnowLine must be less than or equal to SummerSnowLine", [nameof(WinterSnowLine), nameof(SummerSnowLine)]);
		}

		if ((int)FirstSeason >= 4)
		{
			yield return new ValidationResult("FirstSeason must be less than 4", [nameof(FirstSeason)]);
		}
	}
}
