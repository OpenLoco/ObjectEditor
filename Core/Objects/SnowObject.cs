﻿using System.ComponentModel;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;

namespace OpenLocoObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	[LocoStructType(ObjectType.Snow)]
	[LocoStringTable("Name")]
	public record SnowObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] image_id Image
	) : ILocoStruct;
}