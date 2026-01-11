using Definitions.ObjectModels.Objects.Cargo;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;
using System.ComponentModel;

namespace Gui.ViewModels;

public class CargoViewModel(CargoObject model)
		: LocoObjectViewModel<CargoObject>(model)
{
	[Category("Cargo")]
	public uint16_t CargoTransferTime
	{
		get => Model.CargoTransferTime;
		set => Model.CargoTransferTime = value;
	}

	[Category("Cargo")]
	public CargoCategory CargoCategory
	{
		get => Model.CargoCategory;
		set
		{
			if (Model.CargoCategory != value)
			{
				Model.CargoCategory = value;
				this.RaisePropertyChanged(nameof(CargoCategory));
				this.RaisePropertyChanged(nameof(CargoCategoryOverride));
			}
		}
	}

	[Category("Cargo"), Description("Allows the user to set a custom value for the cargo category.")]
	public uint16_t CargoCategoryOverride
	{
		get => (uint16_t)Model.CargoCategory;
		set
		{
			if ((uint16_t)Model.CargoCategory != value)
			{
				Model.CargoCategory = (CargoCategory)value;
				this.RaisePropertyChanged(nameof(CargoCategory));
				this.RaisePropertyChanged(nameof(CargoCategoryOverride));
			}
		}
	}

	[EnumProhibitValues<CargoObjectFlags>(CargoObjectFlags.None)]
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

	public uint16_t UnitWeight
	{
		get => Model.UnitWeight;
		set => Model.UnitWeight = value;
	}
}
