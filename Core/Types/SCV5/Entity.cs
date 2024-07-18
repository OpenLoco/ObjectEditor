using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	public class Entity
	{
		[LocoArrayLength(0x80)] public uint8_t[] pad_0 { get; set; }
	};
}
