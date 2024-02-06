﻿using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x03)]
	public record SimpleAnimation(
		//[property: LocoStructOffset(0x00)] uint8_t ObjectId, // object loader fills this in - not necessary for openlocotool
		[property: LocoStructOffset(0x01)] uint8_t Height,
		[property: LocoStructOffset(0x02)] SimpleAnimationType Type
		) : ILocoStruct;
}
