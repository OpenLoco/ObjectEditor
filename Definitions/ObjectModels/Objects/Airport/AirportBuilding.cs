namespace Definitions.ObjectModels.Objects.Airport;

public class AirportBuilding : ILocoStruct
{
	public uint8_t Index { get; set; }
	public uint8_t Rotation { get; set; }
	public int8_t X { get; set; }
	public int8_t Y { get; set; }

	public bool Validate() => true;
}
