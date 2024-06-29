using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using Zenith.Core;

namespace OpenLoco.ObjectEditor.Headers
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(StructLength)]
	public record S5Header(string Name, uint32_t Checksum)
	{
		// this is necessary because Flags must be get-set to enable setters for SourceGame and ObjectType
		public S5Header(uint32_t flags, string name, uint32_t checksum)
			: this(name, checksum)
			=> Flags = flags;

		public const int StructLength = 0x10;

		public uint32_t Flags { get; set; }

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
		{
			if (SourceGame is < 0 or >= SourceGame.MAX)
			{
				return false;
			}

			if (ObjectType is < 0 or >= ObjectType.MAX)
			{
				return false;
			}

			return true;
		}

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
			var name = System.Text.Encoding.ASCII.GetBytes(Name);
			var checksum = BitConverter.GetBytes(Checksum);

			flags.CopyTo(span, 0);
			name.CopyTo(span, 4);
			checksum.CopyTo(span, 12);
			return span;
		}

		public static S5Header NullHeader = new(0xFFFFFFFF, "        ", 0);
	}
}
