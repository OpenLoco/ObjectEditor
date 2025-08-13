namespace Definitions.ObjectModels.Objects.Currency;
public class CurrencyObject : ILocoStruct
{
	public uint8_t Separator { get; set; }
	public uint8_t Factor { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
