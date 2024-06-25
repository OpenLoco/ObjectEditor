using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	class Station
	{
		[LocoArrayLength(0x3D2)] public uint8_t[] pad_0 { get; set; }
	};
}
