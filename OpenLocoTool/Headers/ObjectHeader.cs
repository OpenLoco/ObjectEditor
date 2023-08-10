using System.ComponentModel;
using System.Text;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Headers
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	public record ObjectHeader(uint32_t Flags, string Name, uint32_t Checksum) : ILocoStruct
	{
		public static int ObjectStructSize => 0x10;

		public byte SourceGame => (byte)(Flags >> 6 & 0x3);
		public ObjectType ObjectType => (ObjectType)(Flags & 0x3F);

		public bool IsCustom => SourceGame == 0;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
		{
			var flags = BitConverter.ToUInt32(data[0..4]);
			var name = Encoding.ASCII.GetString(data[4..12]);
			var checksum = BitConverter.ToUInt32(data[12..16]);
			return new ObjectHeader(flags, name, checksum);
		}

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[ObjectStructSize];

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
