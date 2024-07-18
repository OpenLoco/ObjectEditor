using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	public class Industry
	{
		[LocoArrayLength(0x453)] public uint8_t[] pad_0 { get; set; }
	};
}
