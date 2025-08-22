using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class CurrencyObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{ }

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream stream, LocoObject obj) => throw new NotImplementedException();
}

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x0C)]
[LocoStructType(DatObjectType.Currency)]
internal record DatCurrencyObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), LocoString, Browsable(false)] string_id PrefixSymbol,
	[property: LocoStructOffset(0x04), LocoString, Browsable(false)] string_id SuffixSymbol,
	[property: LocoStructOffset(0x06), Browsable(false)] image_id ObjectIcon,
	[property: LocoStructOffset(0x0A)] uint8_t Separator,
	[property: LocoStructOffset(0x0B)] uint8_t Factor
	)
{
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
}
