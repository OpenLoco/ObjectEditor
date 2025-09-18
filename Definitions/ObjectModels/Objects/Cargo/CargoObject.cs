using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Cargo;

public class CargoObject : ILocoStruct
{
	public uint16_t CargoTransferTime { get; set; }
	public CargoCategory CargoCategory { get; set; }
	public CargoObjectFlags Flags { get; set; }
	public uint8_t NumPlatformVariations { get; set; }
	public uint8_t StationCargoDensity { get; set; }
	public uint8_t PremiumDays { get; set; }
	public uint8_t MaxNonPremiumDays { get; set; }
	public uint16_t MaxPremiumRate { get; set; }
	public uint16_t PenaltyRate { get; set; }
	public uint16_t PaymentFactor { get; set; }
	public uint8_t PaymentIndex { get; set; }
	public uint8_t UnitSize { get; set; }
	public uint16_t var_02 { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
	{
		if (var_02 > 3840)
		{
			yield return new ValidationResult($"{nameof(var_02)} must be less than or equal to 3840.");
		}

		if (CargoTransferTime == 0)
		{
			yield return new ValidationResult($"{nameof(CargoTransferTime)} must be non-zero.");
		}
	}
}
