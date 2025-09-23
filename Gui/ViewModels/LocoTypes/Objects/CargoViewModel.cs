using Definitions.ObjectModels.Objects.Cargo;

namespace Gui.ViewModels;

public class CargoViewModel(CargoObject model)
		: LocoObjectViewModel<CargoObject>(model)
{
	public uint16_t CargoTransferTime
	{
		get => Model.CargoTransferTime;
		set => Model.CargoTransferTime = value;
	}

	public CargoCategory CargoCategory
	{
		get => Model.CargoCategory;
		set => Model.CargoCategory = value;
	}

	public CargoObjectFlags Flags
	{
		get => Model.Flags;
		set => Model.Flags = value;
	}

	public uint8_t NumPlatformVariations
	{
		get => Model.NumPlatformVariations;
		set => Model.NumPlatformVariations = value;
	}

	public uint8_t StationCargoDensity
	{
		get => Model.StationCargoDensity;
		set => Model.StationCargoDensity = value;
	}

	public uint8_t PremiumDays
	{
		get => Model.PremiumDays;
		set => Model.PremiumDays = value;
	}

	public uint8_t MaxNonPremiumDays
	{
		get => Model.MaxNonPremiumDays;
		set => Model.MaxNonPremiumDays = value;
	}

	public uint16_t MaxPremiumRate
	{
		get => Model.NonPremiumRate;
		set => Model.NonPremiumRate = value;
	}

	public uint16_t PenaltyRate
	{
		get => Model.PenaltyRate;
		set => Model.PenaltyRate = value;
	}

	public uint16_t PaymentFactor
	{
		get => Model.PaymentFactor;
		set => Model.PaymentFactor = value;
	}

	public uint8_t PaymentIndex
	{
		get => Model.PaymentIndex;
		set => Model.PaymentIndex = value;
	}

	public uint8_t UnitSize
	{
		get => Model.UnitSize;
		set => Model.UnitSize = value;
	}

	public uint16_t var_02
	{
		get => Model.var_02;
		set => Model.var_02 = value;
	}
}
