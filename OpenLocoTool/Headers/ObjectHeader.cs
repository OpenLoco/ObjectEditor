using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Headers
{
	public enum SourceGame : byte
	{
		Custom = 0,
		Vanilla = 2,
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	public record ObjectHeader(string Name, uint32_t Checksum, SawyerEncoding Encoding, uint32_t DataLength)
	{
		// this is necessary because Flags must be get-set to enable setters for SourceGame and ObjectType
		public ObjectHeader(uint32_t flags, string name, uint32_t checksum, SawyerEncoding encoding, uint32_t dataLength)
			: this(name, checksum, encoding, dataLength)
			=> Flags = flags;

		public static int StructLength => 0x15;

		public uint32_t Flags { get; set; }

		public SourceGame SourceGame
		{
			get => (SourceGame)((Flags >> 6) & 0x3);
			set => Flags = (Flags & ~(0x3u << 6)) | (((uint)value & 0x3u) << 6);
		}

		public ObjectType ObjectType
		{
			get => (ObjectType)(Flags & 0x3F);
			set => Flags = (Flags & ~0x3u) | ((uint)value & 0x3u);
		}

		public static ObjectHeader Read(ReadOnlySpan<byte> data)
		{
			var flags = BitConverter.ToUInt32(data[0..4]);
			var name = System.Text.Encoding.ASCII.GetString(data[4..12]);
			var checksum = BitConverter.ToUInt32(data[12..16]);
			var encoding = (SawyerEncoding)data[16];
			var dataLength = BitConverter.ToUInt32(data[17..21]);
			return new ObjectHeader(flags, name, checksum, encoding, dataLength);
		}

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[StructLength];

			var flags = BitConverter.GetBytes(Flags);
			var name = System.Text.Encoding.ASCII.GetBytes(Name);
			var checksum = BitConverter.GetBytes(Checksum);
			var dataLength = BitConverter.GetBytes(StructLength);

			flags.CopyTo(span, 0);
			name.CopyTo(span, 4);
			checksum.CopyTo(span, 12);
			span[16] = (byte)Encoding;
			dataLength.CopyTo(span, 17);
			return span;
		}
	}
}
