using Dat.FileParsing;

namespace Dat.Types.SCV5;

[LocoStructSize(0x270)]
public class Town : ILocoStruct
{
	[LocoArrayLength(0x270)] public uint8_t[] var_0 { get; set; }

	public bool Validate() => throw new NotImplementedException();
}
