using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	//[LocoStringTable("Name", "PrefixSymbol", "SuffixSymbol")]
	public record CurrencyObject
	(
		//[property: LocoStructOffset(0x00)] string_id Name,
		//[property: LocoStructOffset(0x02)] string_id PrefixSymbol,
		//[property: LocoStructOffset(0x04)] string_id SuffixSymbol,
		[property: LocoStructOffset(0x06)] uint32_t ObjectIcon,
		[property: LocoStructOffset(0x0A)] uint8_t Separator,
		[property: LocoStructOffset(0x0B)] uint8_t Factor
	) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.Currency;

		public static int StructSize => 0x0C;
	}
}
