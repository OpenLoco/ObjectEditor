using Definitions.ObjectModels.Objects.Cargo;

namespace Definitions.Database;

public class TblObjectCargo : DbSubObject, IConvertibleToTable<TblObjectCargo, CargoObject>
{
	public uint16_t CargoTransferTime { get; set; }
	public CargoCategory CargoCategory { get; set; }
	public CargoObjectFlags Flags { get; set; }
	public uint8_t NumPlatformVariations { get; set; }
	public uint8_t StationCargoDensity { get; set; }
	public uint8_t PremiumDays { get; set; }
	public uint8_t MaxNonPremiumDays { get; set; }
	public uint16_t NonPremiumRate { get; set; }
	public uint16_t PenaltyRate { get; set; }
	public uint16_t PaymentFactor { get; set; }
	public uint8_t PaymentIndex { get; set; }
	public uint8_t UnitSize { get; set; }

	//uint16_t var_02 { get; set; }

	public static TblObjectCargo FromObject(TblObject tbl, CargoObject obj)
		=> new()
		{
			Parent = tbl,
			CargoTransferTime = obj.CargoTransferTime,
			CargoCategory = obj.CargoCategory,
			Flags = obj.Flags,
			NumPlatformVariations = obj.NumPlatformVariations,
			StationCargoDensity = obj.StationCargoDensity,
			PremiumDays = obj.PremiumDays,
			MaxNonPremiumDays = obj.MaxNonPremiumDays,
			NonPremiumRate = obj.NonPremiumRate,
			PenaltyRate = obj.PenaltyRate,
			PaymentFactor = obj.PaymentFactor,
			PaymentIndex = obj.PaymentIndex,
			UnitSize = obj.UnitSize,
		};
}
