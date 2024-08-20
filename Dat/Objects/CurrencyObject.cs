using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	[LocoStructType(ObjectType.Currency)]
	[LocoStringTable("Name", "Prefix Symbol", "SuffixSymbol")]
	public record CurrencyObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), LocoString, Browsable(false)] string_id PrefixSymbol,
		[property: LocoStructOffset(0x04), LocoString, Browsable(false)] string_id SuffixSymbol,
		[property: LocoStructOffset(0x06), Browsable(false)] image_id ObjectIcon,
		[property: LocoStructOffset(0x0A)] uint8_t Separator,
		[property: LocoStructOffset(0x0B)] uint8_t Factor
		) : ILocoStruct
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
}
