using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record CompetitorObject(
		[property: LocoStructProperty] string_id var_00,        // 0x00
		[property: LocoStructProperty] string_id var_02,        // 0x02
		[property: LocoStructProperty] uint32_t var_04,         // 0x04
		[property: LocoStructProperty] uint32_t var_08,         // 0x08
		[property: LocoStructProperty] uint32_t Emotions,       // 0x0C
		[property: LocoStructProperty, LocoArrayLength(CompetitorObject.ImagesLength)] uint32_t[] Images, // 0x10
		[property: LocoStructProperty] uint8_t Intelligence,    // 0x34
		[property: LocoStructProperty] uint8_t Aggressiveness,  // 0x35
		[property: LocoStructProperty] uint8_t Competitiveness, // 0x36
		[property: LocoStructProperty] uint8_t var_37           // 0x37
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.competitor;

		public int ObjectStructSize => 0x38;

		public const int ImagesLength = 9;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
		{
			var byteReader = new ByteReader(data);
			return new CompetitorObject(
				byteReader.Read<string_id>(),
				byteReader.Read<string_id>(),
				byteReader.Read<uint32_t>(),
				byteReader.Read<uint32_t>(),
				byteReader.Read<uint32_t>(),
				byteReader.Read_Array<uint32_t>(ImagesLength),
				byteReader.Read<uint8_t>(),
				byteReader.Read<uint8_t>(),
				byteReader.Read<uint8_t>(),
				byteReader.Read<uint8_t>());
		}

		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}

}
