using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Industry;

[TypeConverter(typeof(ExpandableObjectConverter))]

public class IndustryObjectProductionRateRange(uint16_t min, uint16_t max)
{
	public uint16_t Min { get; set; } = min;
	public uint16_t Max { get; set; } = max;
}
