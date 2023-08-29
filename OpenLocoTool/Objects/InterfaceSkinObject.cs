
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x18)]
	public record InterfaceSkinObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint32_t Image,
		[property: LocoStructOffset(0x06)] Colour Colour_06,
		[property: LocoStructOffset(0x07)] Colour Colour_07,
		[property: LocoStructOffset(0x08)] Colour TooltipColour,
		[property: LocoStructOffset(0x09)] Colour ErrorColour,
		[property: LocoStructOffset(0x0A)] Colour Colour_0A,
		[property: LocoStructOffset(0x0B)] Colour Colour_0B,
		[property: LocoStructOffset(0x0C)] Colour Colour_0C,
		[property: LocoStructOffset(0x0D)] Colour Colour_0D,
		[property: LocoStructOffset(0x0E)] Colour Colour_0E,
		[property: LocoStructOffset(0x0F)] Colour Colour_0F,
		[property: LocoStructOffset(0x10)] Colour Colour_10,
		[property: LocoStructOffset(0x11)] Colour Colour_11,
		[property: LocoStructOffset(0x12)] Colour Colour_12,
		[property: LocoStructOffset(0x13)] Colour Colour_13,
		[property: LocoStructOffset(0x14)] Colour Colour_14,
		[property: LocoStructOffset(0x15)] Colour Colour_15,
		[property: LocoStructOffset(0x16)] Colour Colour_16,
		[property: LocoStructOffset(0x17)] Colour Colour_17
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.InterfaceSkin;
		public static int StructSize => 0x18;
	}
}
