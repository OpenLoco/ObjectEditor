using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Industry;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class IndustryObjectUnk38
{
	public uint8_t var_00 { get; set; }
	public uint8_t var_01 { get; set; }
}
