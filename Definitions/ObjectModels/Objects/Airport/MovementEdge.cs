using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Airport;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class MovementEdge : ILocoStruct
{
	public uint8_t CurrNodeType { get; set; } // determines the movement type away from currNode, 2 means take off
	public uint8_t CurrNode { get; set; }
	public uint8_t NextNode { get; set; }
	public uint8_t NextNodeType { get; set; } // determines the status an aircraft adopts on arriving at nextNode
	public uint32_t MustBeClearEdges { get; set; } // which edges must be clear to use the transition edge
	public uint32_t AtLeastOneClearEdges { get; set; } // which edges must have at least one clear to use transition edge

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
