using Dat.FileParsing;
using Definitions.ObjectModels;

namespace Dat.Types.SCV5;

[LocoStructSize(0x06)]
public class Wave : ILocoStruct
{
	[LocoArrayLength(0x06)] public uint8_t[] var_0 { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
