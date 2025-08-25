namespace Definitions.ObjectModels.Objects.Cargo;

public class CargoObject : ILocoStruct, IImageTableNameProvider
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

	public bool Validate()
		=> var_02 <= 3840
		&& CargoTransferTime != 0;

	public bool TryGetImageName(int id, out string? value)
	{
		value = id == 0
			? "kInlineSprite"
			: $"kStationPlatform{id}";

		return true;
	}
}
