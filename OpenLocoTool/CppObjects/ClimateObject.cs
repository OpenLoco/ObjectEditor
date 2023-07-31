using System.ComponentModel;
using System.Text;

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
		uint8_t pad_09) : ILocoSubObject
	{
		public int BinarySize => 0xA;

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

		public ReadOnlySpan<byte> Write()
		{
			var span = new byte[BinarySize];

			var name = Encoding.ASCII.GetBytes(Name);
			// copy to string id table

			span[2] = FirstSeason;
			span[3] = SeasonLength[0];
			span[4] = SeasonLength[1];
			span[5] = SeasonLength[2];
			span[6] = SeasonLength[3];
			span[7] = WinterSnowLine;
			span[8] = SummerSnowLine;
			span[9] = pad_09;

			return span;
		}
	}
}
