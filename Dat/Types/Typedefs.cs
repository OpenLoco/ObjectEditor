using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel.DataAnnotations;

namespace Dat.Types;

[LocoStructSize(0x04)]
public record DatPos2(
	[property: LocoStructOffset(0x00)] coord_t X = 0,
	[property: LocoStructOffset(0x02)] coord_t Y = 0
	) : ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}

[LocoStructSize(0x06)]
public record DatPos3(
	[property: LocoStructOffset(0x00)] coord_t X = 0,
	[property: LocoStructOffset(0x02)] coord_t Y = 0,
	[property: LocoStructOffset(0x04)] coord_t Z = 0
	) : ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
