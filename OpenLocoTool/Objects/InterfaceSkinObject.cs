using System.ComponentModel;
using OpenLocoTool.Data;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x18)]
	[LocoStructType(ObjectType.InterfaceSkin)]
	[LocoStringTable("Name")]
	public class InterfaceSkinObject(
		Colour colour_06,
		Colour colour_07,
		Colour tooltipColour,
		Colour errorColour,
		Colour colour_0A,
		Colour colour_0B,
		Colour colour_0C,
		Colour colour_0D,
		Colour colour_0E,
		Colour colour_0F,
		Colour colour_10,
		Colour colour_11,
		Colour colour_12,
		Colour colour_13,
		Colour colour_14,
		Colour colour_15,
		Colour colour_16,
		Colour colour_17)
		: ILocoStruct
	{

		//[LocoStructOffset(0x02)] public image_id Image { get; set; }
		[LocoStructOffset(0x06)] public Colour Colour_06 { get; set; } = colour_06;
		[LocoStructOffset(0x07)] public Colour Colour_07 { get; set; } = colour_07;
		[LocoStructOffset(0x08)] public Colour TooltipColour { get; set; } = tooltipColour;
		[LocoStructOffset(0x09)] public Colour ErrorColour { get; set; } = errorColour;
		[LocoStructOffset(0x0A)] public Colour Colour_0A { get; set; } = colour_0A;
		[LocoStructOffset(0x0B)] public Colour Colour_0B { get; set; } = colour_0B;
		[LocoStructOffset(0x0C)] public Colour Colour_0C { get; set; } = colour_0C;
		[LocoStructOffset(0x0D)] public Colour Colour_0D { get; set; } = colour_0D;
		[LocoStructOffset(0x0E)] public Colour Colour_0E { get; set; } = colour_0E;
		[LocoStructOffset(0x0F)] public Colour Colour_0F { get; set; } = colour_0F;
		[LocoStructOffset(0x10)] public Colour Colour_10 { get; set; } = colour_10;
		[LocoStructOffset(0x11)] public Colour Colour_11 { get; set; } = colour_11;
		[LocoStructOffset(0x12)] public Colour Colour_12 { get; set; } = colour_12;
		[LocoStructOffset(0x13)] public Colour Colour_13 { get; set; } = colour_13;
		[LocoStructOffset(0x14)] public Colour Colour_14 { get; set; } = colour_14;
		[LocoStructOffset(0x15)] public Colour Colour_15 { get; set; } = colour_15;
		[LocoStructOffset(0x16)] public Colour Colour_16 { get; set; } = colour_16;
		[LocoStructOffset(0x17)] public Colour Colour_17 { get; set; } = colour_17;
	}
}
