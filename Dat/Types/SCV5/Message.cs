using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	public class Message
	{
		[LocoArrayLength(0xD4)] public uint8_t[] var_0 { get; set; }
	};
}
