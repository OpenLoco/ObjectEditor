using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel.DataAnnotations;

namespace Dat.Types.SCV5;

[LocoStructSize(0x270)]
public class Town : ILocoStruct
{
	[LocoArrayLength(0x270)] public uint8_t[] var_0 { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
