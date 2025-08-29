using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.TownNames;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class Category
{
	public uint8_t Count { get; set; }
	public uint8_t Bias { get; set; }
	public uint16_t Offset { get; set; }
}
