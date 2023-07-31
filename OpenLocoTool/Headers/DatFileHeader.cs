using System.ComponentModel;
using System.Text;

namespace OpenLocoTool
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	public record DatFileHeader(uint32_t Flags, string Name, uint32_t Checksum) : ILocoSubObject
	{
		public int BinarySize => 0x10;

		public byte SourceGame => (byte)((Flags >> 6) & 0x3);
		public ObjectType ObjectType => (ObjectType)(Flags & 0x3F);

		public bool IsCustom => SourceGame == 0;

		public static DatFileHeader Read(ReadOnlySpan<byte> data)
		{
			var flags = BitConverter.ToUInt32(data[0..4]);
			var name = Encoding.ASCII.GetString(data[4..12]);
			var checksum = BitConverter.ToUInt32(data[12..16]);
			return new DatFileHeader(flags, name, checksum);
		}

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[BinarySize];

			var flags = BitConverter.GetBytes(Flags);
			var name = Encoding.ASCII.GetBytes(Name);
			var checksum = BitConverter.GetBytes(Checksum);

			flags.CopyTo(span, 0);
			name.CopyTo(span, 4);
			checksum.CopyTo(span, 12);

			return span;
		}
	}
}
