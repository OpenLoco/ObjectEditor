using System.ComponentModel.DataAnnotations;

namespace Definitions.ObjectModels.Objects.Wall;

public class WallObject : ILocoStruct
{
	public uint8_t ToolId { get; set; } // unused in loco???
	public WallObjectFlags1 Flags1 { get; set; } = WallObjectFlags1.None;
	public uint8_t Height { get; set; }
	public WallObjectFlags2 Flags2 { get; set; } = WallObjectFlags2.None; // unused in loco???

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
