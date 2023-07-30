using System.ComponentModel;
using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	// size = 0xA
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record ClimateObject(string Name, uint8_t FirstSeason, uint8_t[] SeasonLength, uint8_t WinterSnowLine, uint8_t SummerSnowLine, uint8_t Pad_09);
}
