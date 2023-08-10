using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record CompetitorObject(
		[property: LocoStructProperty(0x00)] string_id var_00,
		[property: LocoStructProperty(0x02)] string_id var_02,
		[property: LocoStructProperty(0x04)] uint32_t var_04,
		[property: LocoStructProperty(0x08)] uint32_t var_08,
		[property: LocoStructProperty(0x0C)] uint32_t Emotions,
		[property: LocoStructProperty(0x10), LocoArrayLength(CompetitorObject.ImagesLength)] uint32_t[] Images,
		[property: LocoStructProperty(0x34)] uint8_t Intelligence,
		[property: LocoStructProperty(0x35)] uint8_t Aggressiveness,
		[property: LocoStructProperty(0x36)] uint8_t Competitiveness,
		[property: LocoStructProperty(0x37)] uint8_t var_37
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.competitor;

		public static int ObjectStructSize => 0x38;

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
