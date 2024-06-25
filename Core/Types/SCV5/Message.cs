using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	public class Message
	{
		[LocoArrayLength(0xD4)] public uint8_t[] pad_0 { get; set; }
	};
}
