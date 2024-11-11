using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	public class Industry
	{
		[LocoArrayLength(0x453)] public uint8_t[] var_0 { get; set; }
	};
}
