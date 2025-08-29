namespace Definitions.ObjectModels.Objects.Airport;

public class MovementNode : ILocoStruct
{
	public int16_t X { get; set; }
	public int16_t Y { get; set; }
	public int16_t Z { get; set; }
	public AirportMovementNodeFlags Flags { get; set; }

	public bool Validate() => true;
}
