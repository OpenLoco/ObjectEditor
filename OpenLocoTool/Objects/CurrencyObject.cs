using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0C)]
	[LocoStructType(ObjectType.Currency)]
	[LocoStringTable("Name", "Prefix Symbol", "SuffixSymbol")]
	public class CurrencyObject(
		uint8_t separator,
		uint8_t factor)
		: ILocoStruct
	{

		//[property: LocoStructOffset(0x00), LocoString, Browsable(false)] public string_id Name,
		//[property: LocoStructOffset(0x02), LocoString, Browsable(false)] public string_id PrefixSymbol,
		//[property: LocoStructOffset(0x04), LocoString, Browsable(false)] public string_id SuffixSymbol,
		//[LocoStructOffset(0x06)] public image_id ObjectIcon { get; set; } = objectIcon;
		[LocoStructOffset(0x0A)] public uint8_t Separator { get; set; } = separator;
		[LocoStructOffset(0x0B)] public uint8_t Factor { get; set; } = factor;
	}
}
