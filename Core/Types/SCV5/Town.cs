using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	class Town
	{
		[LocoArrayLength(0x270)] public uint8_t[] pad_0 { get; set; }
	};
}
