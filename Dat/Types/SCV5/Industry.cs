using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel.DataAnnotations;

namespace Dat.Types.SCV5;

[LocoStructSize(0x453)]
public class Industry : ILocoStruct
{
	[LocoArrayLength(0x453)] public uint8_t[] var_0 { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
