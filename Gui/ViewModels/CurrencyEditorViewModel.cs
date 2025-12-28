using ReactiveUI;

namespace Gui.ViewModels;

public class CurrencyEditorViewModel : ReactiveObject
{
	private short _costFactor;
	private byte _costIndex;
	private int _year = 1950;

	public short CostFactor
	{
		get => _costFactor;
		set
		{
			this.RaiseAndSetIfChanged(ref _costFactor, value);
			this.RaisePropertyChanged(nameof(EffectiveCost));
		}
	}

	public byte CostIndex
	{
		get => _costIndex;
		set
		{
			this.RaiseAndSetIfChanged(ref _costIndex, value);
			this.RaisePropertyChanged(nameof(EffectiveCost));
		}
	}

	public int Year
	{
		get => _year;
		set
		{
			this.RaiseAndSetIfChanged(ref _year, value);
			this.RaisePropertyChanged(nameof(EffectiveCost));
		}
	}

	public int EffectiveCost
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
