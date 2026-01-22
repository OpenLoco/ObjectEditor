using Definitions.ObjectModels.Objects.Currency;

namespace Gui.ViewModels;

public class CurrencyViewModel(CurrencyObject model)
		: LocoObjectViewModel<CurrencyObject>(model)
{
	public uint8_t Separator
	{
		get => Model.Separator;
		set => Model.Separator = value;
	}

	public uint8_t Factor
	{
		get => Model.Factor;
		set => Model.Factor = value;
	}
}
