using System.Diagnostics.CodeAnalysis;

namespace Definitions.ObjectModels.Objects.Currency;

public class CurrencyObject : ILocoStruct, IImageTableNameProvider
{
	public uint8_t Separator { get; set; }
	public uint8_t Factor { get; set; }

	public bool Validate()
	{
		if (Separator > 4)
		{
			return false;
		}

		if (Factor > 3)
		{
			return false;
		}

		return true;
	}

	public bool TryGetImageName(int id, [MaybeNullWhen(false)] out string value)
		=> throw new NotImplementedException();
}
