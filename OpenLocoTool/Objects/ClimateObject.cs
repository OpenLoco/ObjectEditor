using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0A)]
	[LocoStringCount(1)]
	public record ClimateObject(
		//[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t FirstSeason,
		[property: LocoStructOffset(0x03), LocoArrayLength(ClimateObject.Seasons)] uint8_t[] SeasonLengths,
		[property: LocoStructOffset(0x07)] uint8_t WinterSnowLine,
		[property: LocoStructOffset(0x08)] uint8_t SummerSnowLine,
		[property: LocoStructOffset(0x09)] uint8_t pad_09
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.Climate;

		public static int StructSize => 0x0A;

		public const int Seasons = 4;
	}
}
