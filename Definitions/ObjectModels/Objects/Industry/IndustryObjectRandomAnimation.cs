using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Industry;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class IndustryObjectRandomAnimation
{
	public uint8_t BuildingPart { get; set; } // building part the animation can be applied to
	public uint8_t AnimationIndex { get; set; } // animation sequence index to use
}