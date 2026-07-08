using Definitions;
using ReactiveUI;
using System;

namespace Gui.ViewModels;

public class InflatableCurrencyViewModel : ReactiveObject
{
	public const int DefaultYear = 1950;

	public short CostFactor
	{
		get;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref field, value);
			this.RaisePropertyChanged(nameof(InflationAdjustedCost));
		}
	}

	public byte CostIndex
	{
		get;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref field, value);
			this.RaisePropertyChanged(nameof(InflationAdjustedCost));
		}
	}

	public int Year
	{
		get;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref field, value);
			this.RaisePropertyChanged(nameof(InflationAdjustedCost));
		}
	} = DefaultYear;

	public byte Divisor
	{
		get;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref field, value);
			this.RaisePropertyChanged(nameof(InflationAdjustedCost));
		}
	}

	public int ExchangeRate
	{
		get;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref field, value);
			this.RaisePropertyChanged(nameof(InflationAdjustedCost));
		}
	}

	public int InflationAdjustedCost
		=> CostIndex >= 32
			? 0
			: Economy.GetInflationAdjustedCost(CostFactor, CostIndex, Year, Divisor) * (int)Math.Pow(2, Math.Max(0, ExchangeRate));
}
