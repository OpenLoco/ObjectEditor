using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record ClimateObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint8_t FirstSeason,
		[property: LocoStructProperty(0x03), LocoArrayLength(ClimateObject.Seasons)] uint8_t[] SeasonLengths,
		[property: LocoStructProperty(0x07)] uint8_t WinterSnowLine,
		[property: LocoStructProperty(0x08)] uint8_t SummerSnowLine,
		[property: LocoStructProperty(0x09)] uint8_t pad_09
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.climate;

		public static int StructLength => 0xA;

		public const int Seasons = 4;
	}
}
