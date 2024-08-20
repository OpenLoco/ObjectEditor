using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using System.ComponentModel;
using Zenith.Core;

namespace OpenLoco.Dat.Types
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(StructLength)]
	public record S5Header
	{
		// this is necessary because Flags must be get-set to enable setters for SourceGame and ObjectType
		public S5Header(uint32_t flags, string name, uint32_t checksum)
		{
			Flags = flags;
			Name = name.Trim();
			Checksum = checksum;
		}

		public S5Header(string name, uint32_t checksum)
			: this(0, name, checksum)
		{ }

		public const int StructLength = 0x10;

		public uint32_t Flags { get; set; }
		public string Name { get; set; }
		public uint32_t Checksum { get; set; }

		public SourceGame SourceGame
		{
			get => (SourceGame)((Flags >> 6) & 0x3u);
			set => Flags = (Flags & ~(0x3u << 6)) | (((uint)value & 0x3u) << 6);
		}

		public ObjectType ObjectType
		{
			get => (ObjectType)(Flags & 0x3F);
			set => Flags = (Flags & (~0x3u << 6)) | ((uint)value & 0x3F);
		}

		public bool Validate()
			=> (int)SourceGame is >= 0 and < 3 && (int)ObjectType is >= 0 and < Limits.kMaxObjectTypes;

		public static S5Header Read(ReadOnlySpan<byte> data)
		{
			Verify.AreEqual(data.Length, StructLength);

			var flags = BitConverter.ToUInt32(data[0..4]);
			var name = System.Text.Encoding.ASCII.GetString(data[4..12]);
			var checksum = BitConverter.ToUInt32(data[12..16]);
			return new S5Header(flags, name, checksum);
		}

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[StructLength];

			var flags = BitConverter.GetBytes(Flags);
			var name = System.Text.Encoding.ASCII.GetBytes(Name.PadRight(8, ' '));
			var checksum = BitConverter.GetBytes(Checksum);

			flags.CopyTo(span, 0);
			name.CopyTo(span, 4);
			checksum.CopyTo(span, 12);
			return span;
		}

		// Vanilla objects do 0x000000FF but all FF is fine too
		public static readonly S5Header NullHeader = new(0x000000FF, "        ", 0);
	}
}
