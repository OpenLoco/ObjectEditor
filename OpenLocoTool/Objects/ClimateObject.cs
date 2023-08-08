using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record ClimateObject(
		[property: LocoStructProperty] string_id Name,         // 0x00
		[property: LocoStructProperty] uint8_t FirstSeason,    // 0x02
		[property: LocoStructProperty, LocoArrayLength(ClimateObject.Seasons)] uint8_t[] SeasonLengths, // 0x03
		[property: LocoStructProperty] uint8_t WinterSnowLine, // 0x07
		[property: LocoStructProperty] uint8_t SummerSnowLine, // 0x08
		[property: LocoStructProperty] uint8_t pad_09) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.climate;

		public int ObjectStructSize => 0xA;

		public const int Seasons = 4;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
		{
			var name = "todo: implement code to lookup string table";
			var byteReader = new ByteReader(data);
			return new ClimateObject(
				byteReader.Read<string_id>(),
				byteReader.Read<uint8_t>(),
				byteReader.Read_Array<uint8_t>(4),
				byteReader.Read<uint8_t>(),
				byteReader.Read<uint8_t>(),
				byteReader.Read<uint8_t>());
		}

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[ObjectStructSize];

			//var name = Encoding.ASCII.GetBytes(Name);
			// copy to string id table

			span[2] = FirstSeason;
			span[3] = SeasonLengths[0];
			span[4] = SeasonLengths[1];
			span[5] = SeasonLengths[2];
			span[6] = SeasonLengths[3];
			span[7] = WinterSnowLine;
			span[8] = SummerSnowLine;
			span[9] = pad_09;

			return span;
		}

	}
}
