﻿using System.ComponentModel;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;

namespace OpenLocoObjectEditor.Objects
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
	) : ILocoStruct;
}