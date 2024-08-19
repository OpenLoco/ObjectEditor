using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	public class Town
	{
		[LocoArrayLength(0x270)] public uint8_t[] pad_0 { get; set; }
	};
}
