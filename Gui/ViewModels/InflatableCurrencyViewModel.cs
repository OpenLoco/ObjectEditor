using Definitions;
using ReactiveUI;

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

	public int InflationAdjustedCost
		=> CostIndex >= 32
			? 0
			: Economy.GetInflationAdjustedCost(CostFactor, CostIndex, Year);
}
