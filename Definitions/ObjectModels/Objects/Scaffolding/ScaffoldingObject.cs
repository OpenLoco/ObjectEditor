using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Scaffolding;
public class ScaffoldingObject : ILocoStruct
{
	public List<uint16_t> SegmentHeights { get; set; } = [];
	public List<uint16_t> RoofHeights { get; set; } = [];

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
