using ReactiveUI;

namespace Gui.ViewModels;

public class InflatableCurrencyViewModel : ReactiveObject
{
	private short _costFactor;
	private byte _costIndex;
	private int _year = 1950;

	public short CostFactor
	{
		get => _costFactor;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref _costFactor, value);
			this.RaisePropertyChanged(nameof(InflationAdjustedCost));
		}
	}

	public byte CostIndex
	{
		get => _costIndex;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref _costIndex, value);
			this.RaisePropertyChanged(nameof(InflationAdjustedCost));
		}
	}

	public int Year
	{
		get => _year;
		set
		{
			_ = this.RaiseAndSetIfChanged(ref _year, value);
			this.RaisePropertyChanged(nameof(InflationAdjustedCost));
		}
	}

	public int InflationAdjustedCost
	{
		get
		{
			if (_costIndex >= 32)
			{
				return 0;
			}
			return Common.Economy.GetInflationAdjustedCost(_costFactor, _costIndex, _year);
		}
	}
}
