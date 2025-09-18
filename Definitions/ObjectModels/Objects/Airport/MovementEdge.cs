using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Airport;

[TypeConverter(typeof(ExpandableObjectConverter))]
public class MovementEdge : ILocoStruct
{
	public uint8_t var_00 { get; set; }
	public uint8_t CurrNode { get; set; }
	public uint8_t NextNode { get; set; }
	public uint8_t var_03 { get; set; }
	public uint32_t MustBeClearEdges { get; set; } // Which edges must be clear to use the transition edge. should probably be some kind of flags?
	public uint32_t AtLeastOneClearEdges { get; set; } // Which edges must have at least one clear to use transition edge. should probably be some kind of flags?

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
