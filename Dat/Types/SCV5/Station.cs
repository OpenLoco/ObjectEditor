using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel.DataAnnotations;

namespace Dat.Types.SCV5;

[LocoStructSize(0x3D2)]
public class Station : ILocoStruct
{
	[LocoArrayLength(0x3D2)] public uint8_t[] var_0 { get; set; }

	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
