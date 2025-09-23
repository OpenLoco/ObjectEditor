using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Scaffolding;

public class ScaffoldingObject : ILocoStruct
{
	[Length(3, 3)]
	public List<uint16_t> SegmentHeights { get; set; } = [];

	[Length(3, 3)]
	public List<uint16_t> RoofHeights { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
