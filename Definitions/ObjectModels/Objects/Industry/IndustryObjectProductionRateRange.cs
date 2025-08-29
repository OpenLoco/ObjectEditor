using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Industry;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class IndustryObjectProductionRateRange
{
	public uint16_t Min { get; set; }
	public uint16_t Max { get; set; }
}
