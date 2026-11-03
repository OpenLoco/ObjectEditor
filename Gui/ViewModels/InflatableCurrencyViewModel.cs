using Definitions;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace Gui.ViewModels;

public class InflatableCurrencyViewModel : ReactiveObject
{
	public const int DefaultYear = 1950;

	readonly ObservableAsPropertyHelper<int> inflationAdjustedCost;

	public InflatableCurrencyViewModel()
	{
		inflationAdjustedCost = this.WhenAnyValue(
				o => o.CostFactor,
				o => o.CostIndex,
				o => o.Year,
				o => o.Divisor,
				o => o.ExchangeRate,
				(costFactor, costIndex, year, divisor, exchangeRate) => costIndex >= 32
					? 0
					: Economy.GetInflationAdjustedCost(costFactor, costIndex, year, divisor) * (int)Math.Pow(2, Math.Max(0, exchangeRate)))
			.ToProperty(this, o => o.InflationAdjustedCost);
	}

	public short CostFactor
	{
		get;
		set => _ = this.RaiseAndSetIfChanged(ref field, value);
	}

	public byte CostIndex
	{
		get;
		set => _ = this.RaiseAndSetIfChanged(ref field, value);
	}

	public int Year
	{
		get;
		set => _ = this.RaiseAndSetIfChanged(ref field, value);
	} = DefaultYear;

	public byte Divisor
	{
		get;
		set => _ = this.RaiseAndSetIfChanged(ref field, value);
	}

	public int ExchangeRate
	{
		get;
		set => _ = this.RaiseAndSetIfChanged(ref field, value);
	}

	public int InflationAdjustedCost
		=> inflationAdjustedCost.Value;
}
