using Definitions.Database;
using Definitions.ObjectModels.Objects.Cargo;

namespace Definitions.DTO;

public class DtoObjectCargo : IDtoSubObject
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
	public UniqueObjectId Id { get; set; }
}
