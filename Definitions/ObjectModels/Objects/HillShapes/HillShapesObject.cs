using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.HillShapes;

public class HillShapesObject : ILocoStruct
{
	public uint8_t HillHeightMapCount { get; set; }
	public uint8_t MountainHeightMapCount { get; set; }
	public bool IsHeightMap { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
