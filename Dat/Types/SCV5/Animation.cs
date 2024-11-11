using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	public class Animation
	{
		[LocoArrayLength(0x06)] public uint8_t[] var_0 { get; set; }
	};
}
