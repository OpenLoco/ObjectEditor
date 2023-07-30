using System.ComponentModel;

namespace OpenLocoTool
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record DatFileHeader(uint32_t Flags, string Name, uint32_t Checksum)
	{
		public byte SourceGame => (byte)((Flags >> 6) & 0x3);
		public ObjectType ObjectType => (ObjectType)(Flags & 0x3F);

		public bool IsCustom => SourceGame == 0;
	}
}
