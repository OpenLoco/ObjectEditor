﻿
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum WallObjectFlags : uint8_t
	{
		none = 0,
		hasPrimaryColour = 1 << 0,
		unk1 = 1 << 1,
		onlyOnLevelLand = 1 << 2,
		unk3 = 1 << 3,
		unk4 = 1 << 4,
		unk5 = 1 << 5,
		hasSecondaryColour = 1 << 6,
		hasTertiaryColour = 1 << 7,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0A)]
	public record WallObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint32_t Image,
		[property: LocoStructOffset(0x06)] uint8_t var_06,
		[property: LocoStructOffset(0x07)] WallObjectFlags Flags,
		[property: LocoStructOffset(0x08)] uint8_t Height,
		[property: LocoStructOffset(0x09)] uint8_t var_09
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.wall;
		public static int StructLength => 0x0A;
	}
}