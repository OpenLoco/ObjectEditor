using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	[LocoStructSize(0x20)]
	class Header
	{
		[LocoStructOffset(0x00)] public S5Type Type { get; set; }
		[LocoStructOffset(0x01)] public HeaderFlags Flags { get; set; }
		[LocoStructOffset(0x02)] public uint16_t NumPackedObjects { get; set; }
		[LocoStructOffset(0x04)] public uint32_t Version { get; set; }
		[LocoStructOffset(0x08)] public uint32_t Magic { get; set; }
		[LocoStructOffset(0x0C), LocoArrayLength(20)] public byte[] Padding { get; set; }
	}
}
