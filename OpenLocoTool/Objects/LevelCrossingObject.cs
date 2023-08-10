
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record LevelCrossingObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] int16_t CostFactor,
		[property: LocoStructProperty(0x04)] int16_t SellCostFactor,
		[property: LocoStructProperty(0x06)] uint8_t CostIndex,
		[property: LocoStructProperty(0x07)] uint8_t AnimationSpeed,
		[property: LocoStructProperty(0x08)] uint8_t ClosingFrames,
		[property: LocoStructProperty(0x09)] uint8_t ClosedFrames,
		[property: LocoStructProperty(0x0A), LocoArrayLength(0x0C - 0x0A)] uint8_t pad_0A,
		[property: LocoStructProperty(0x0C)] uint16_t DesignedYear,
		[property: LocoStructProperty(0x0E)] uint32_t Image
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.levelCrossing;
		public static int ObjectStructSize => 0x12;
	}
}
