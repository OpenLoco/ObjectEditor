using System.ComponentModel;

namespace OpenLocoTool.Objects
{
	// size = 0xA
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record ClimateObject(
		string Name,            // 0x00
		uint8_t FirstSeason,    // 0x02
		uint8_t[] SeasonLength, // 0x03
		uint8_t WinterSnowLine, // 0x07
		uint8_t SummerSnowLine, // 0x08
		uint8_t pad_09)
	{
		public static ClimateObject Read(ReadOnlySpan<byte> data)
		{
			var name = "todo: implement code to lookup string table";
			var firstSeason = data[2];

			var seasonLength = new uint8_t[4];
			seasonLength[0] = data[3];
			seasonLength[1] = data[4];
			seasonLength[2] = data[5];
			seasonLength[3] = data[6];

			var winterSnowLine = data[7];
			var summerSnowLine = data[8];
			uint8_t pad_09 = 0;

			return new ClimateObject(name, firstSeason, seasonLength, winterSnowLine, summerSnowLine, pad_09);
		}
	}
}
