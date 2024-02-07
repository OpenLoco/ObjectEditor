using System.ComponentModel;
using OpenLocoObjectEditor.DatFileParsing;

namespace Core.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x02)]

	public record BuildingPartAnimation(
		[property: LocoStructOffset(0x00)] uint8_t NumFrames,     // Must be a power of 2 (0 = no part animation, could still have animation sequence)
		[property: LocoStructOffset(0x01)] uint8_t AnimationSpeed // Also encodes in bit 7 if the animation is position modified
		) : ILocoStruct;
}
