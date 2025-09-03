using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Airport;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class AirportBuilding : ILocoStruct
{
	public uint8_t Index { get; set; }
	public uint8_t Rotation { get; set; }
	public int8_t X { get; set; }
	public int8_t Y { get; set; }

	public bool Validate() => true;
}
