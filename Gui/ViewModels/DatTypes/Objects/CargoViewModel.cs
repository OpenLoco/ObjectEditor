using Definitions.ObjectModels.Objects.Cargo;

namespace Gui.ViewModels;

public class CargoViewModel : LocoObjectViewModel<CargoObject>
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

	public CargoViewModel(CargoObject obj)
	{
		CargoTransferTime = obj.CargoTransferTime;
		CargoCategory = obj.CargoCategory;
		Flags = obj.Flags;
		NumPlatformVariations = obj.NumPlatformVariations;
		StationCargoDensity = obj.StationCargoDensity;
		PremiumDays = obj.PremiumDays;
		MaxNonPremiumDays = obj.MaxNonPremiumDays;
		MaxPremiumRate = obj.MaxPremiumRate;
		PenaltyRate = obj.PenaltyRate;
		PaymentFactor = obj.PaymentFactor;
		PaymentIndex = obj.PaymentIndex;
		UnitSize = obj.UnitSize;
		var_02 = obj.var_02;
	}

	public override CargoObject GetAsModel()
		=> new()
		{
			CargoTransferTime = CargoTransferTime,
			CargoCategory = CargoCategory,
			Flags = Flags,
			NumPlatformVariations = NumPlatformVariations,
			StationCargoDensity = StationCargoDensity,
			PremiumDays = PremiumDays,
			MaxNonPremiumDays = MaxNonPremiumDays,
			MaxPremiumRate = MaxPremiumRate,
			PenaltyRate = PenaltyRate,
			PaymentFactor = PaymentFactor,
			PaymentIndex = PaymentIndex,
			UnitSize = UnitSize,
			var_02 = var_02,
		};
}
