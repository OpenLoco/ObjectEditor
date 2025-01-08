using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	[LocoStructSize(0x80)]
	public class Entity
	{
		[LocoArrayLength(0x80)] public uint8_t[] var_0 { get; set; }
	};
}
