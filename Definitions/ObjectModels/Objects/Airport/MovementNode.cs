using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Definitions.ObjectModels.Objects.Airport;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class MovementNode : ILocoStruct
{
	public Pos3 Position { get; set; }
	public AirportMovementNodeFlags Flags { get; set; }

	public bool Validate() => true;
}
