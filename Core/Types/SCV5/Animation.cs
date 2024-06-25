using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	class Animation
	{
		[LocoArrayLength(0x06)] public uint8_t[] pad_0 { get; set; }
	};
}
