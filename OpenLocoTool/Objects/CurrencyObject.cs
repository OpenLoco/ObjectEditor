using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	public record CurrencyObject
	(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] string_id PrefixSymbol,
		[property: LocoStructProperty(0x04)] string_id SuffixSymbol,
		[property: LocoStructProperty(0x06)] uint32_t ObjectIcon,
		[property: LocoStructProperty(0x0A)] uint8_t Separator,
		[property: LocoStructProperty(0x0B)] uint8_t Factor
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.currency;

		public static int StructLength => 0x0C;
	}
}
