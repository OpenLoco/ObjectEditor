using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	public class Station
	{
		[LocoArrayLength(0x3D2)] public uint8_t[] var_0 { get; set; }
	};
}
