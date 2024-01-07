using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0A)]
	[LocoStructType(ObjectType.Climate)]
	[LocoStringTable("Name")]
	public class ClimateObject(
		byte firstSeason,
		byte[] seasonLengths,
		byte winterSnowLine,
		byte summerSnowLine)
		: ILocoStruct
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name { get; set; }
		[LocoStructOffset(0x02)] public uint8_t FirstSeason { get; set; } = firstSeason;
		[LocoStructOffset(0x03), LocoArrayLength(Seasons)] public uint8_t[] SeasonLengths { get; set; } = seasonLengths;
		[LocoStructOffset(0x07)] public uint8_t WinterSnowLine { get; set; } = winterSnowLine;
		[LocoStructOffset(0x08)] public uint8_t SummerSnowLine { get; set; } = summerSnowLine;
		//[LocoStructOffset(0x09)] uint8_t pad_09 { get; set; }

		public const int Seasons = 4;
	}
}
