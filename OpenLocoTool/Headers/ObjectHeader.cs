using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Objects;

namespace OpenLocoTool.Headers
{
	public enum SourceGame : byte
	{
		Custom = 0,
		Vanilla = 2,
	}

	[Flags]
	public enum G1ElementFlags : uint16_t
	{
		None = 0,
		HasTransparency = 1 << 0,   // Image data contains transparent sections (when not set data is plain bmp)
		unk1 = 1 << 1,              // Unknown function not used on any entry
		IsRLECompressed = 1 << 2,   // Image data is encoded using CS's form of run length encoding
		IsR8G8B8Palette = 1 << 3,   // Image data is a sequence of palette entries R8G8B8
		HasZoomSprites = 1 << 4,    // Use a different sprite for higher zoom levels
		NoZoomDraw = 1 << 5,        // Does not get drawn at higher zoom levels (only zoom 0)
		DuplicatePrevious = 1 << 6, // Duplicates the previous element but with adjusted x/y offsets
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	[LocoStructSize(0x06)]
	public record StringTableResult(
		[property: LocoStructOffset(0x00)] string_id Str,
		[property: LocoStructOffset(0x04)] uint32_t TableLength);

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	[LocoStructSize(0x08)]
	public record G1Header(
		[property: LocoStructOffset(0x00)] uint32_t NumEntries,
		[property: LocoStructOffset(0x04)] uint32_t TotalSize
		) : ILocoStruct
	{
		public byte[] ImageData;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	[LocoStructSize(0x10)]
	public record G1Element32(
		[property: LocoStructOffset(0x00)] uint32_t offset,
		[property: LocoStructOffset(0x04)] int16_t width,
		[property: LocoStructOffset(0x06)] int16_t height,
		[property: LocoStructOffset(0x08)] int16_t xOffset,
		[property: LocoStructOffset(0x0A)] int16_t yOffset,
		[property: LocoStructOffset(0x0C)] G1ElementFlags flags,
		[property: LocoStructOffset(0x0E)] int16_t zoomOffset
	) : ILocoStruct
	{
		public byte[] ImageData;
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	[LocoStructSize(0x15)]
	public record ObjectHeader(string Name, uint32_t Checksum, SawyerEncoding Encoding, uint32_t DataLength)
	{
		// this is necessary because Flags must be get-set to enable setters for SourceGame and ObjectType
		public ObjectHeader(uint32_t flags, string name, uint32_t checksum, SawyerEncoding encoding, uint32_t dataLength)
			: this(name, checksum, encoding, dataLength)
			=> Flags = flags;

		public static int StructLength => 0x15;
		public static int SubHeaderLength => 0x10;

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
