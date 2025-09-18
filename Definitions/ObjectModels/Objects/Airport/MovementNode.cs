using Definitions.ObjectModels.Types;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Airport;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class MovementNode : ILocoStruct
{
	public Pos3 Position { get; set; }
	public AirportMovementNodeFlags Flags { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
