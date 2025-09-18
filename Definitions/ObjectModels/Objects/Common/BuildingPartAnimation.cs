using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Common;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class BuildingPartAnimation
{
	public uint8_t NumFrames { get; set; } // Must be a power of 2 (0 = no part animation, could still have animation sequence)
	public uint8_t AnimationSpeed { get; set; } // Also encodes in bit 7 if the animation is position modified

	public bool Validate()
		=> IsPowerOfTwo(NumFrames);

	static bool IsPowerOfTwo(uint8_t x)
		=> (x & (x - 1)) == 0 && x > 0;
}
