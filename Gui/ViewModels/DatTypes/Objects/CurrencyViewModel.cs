using Definitions.ObjectModels.Objects.Currency;

namespace Gui.ViewModels;

public class CurrencyViewModel : LocoObjectViewModel<CurrencyObject>
{
	public uint8_t Separator { get; set; }
	public uint8_t Factor { get; set; }

	public CurrencyViewModel(CurrencyObject obj)
	{
		Separator = obj.Separator;
		Factor = obj.Factor;
	}

	public override CurrencyObject GetAsModel()
		=> new()
		{
			Separator = Separator,
			Factor = Factor,
		};
}
