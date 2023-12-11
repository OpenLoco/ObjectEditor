
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	//[LocoStringTable("Name")]
	public record LevelCrossingObject(
		//[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] int16_t CostFactor,
		[property: LocoStructOffset(0x04)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x06)] uint8_t CostIndex,
		[property: LocoStructOffset(0x07)] uint8_t AnimationSpeed,
		[property: LocoStructOffset(0x08)] uint8_t ClosingFrames,
		[property: LocoStructOffset(0x09)] uint8_t ClosedFrames,
		[property: LocoStructOffset(0x0A), LocoArrayLength(0x0C - 0x0A)] uint8_t[] pad_0A,
		[property: LocoStructOffset(0x0C)] uint16_t DesignedYear
		//[property: LocoStructOffset(0x0E)] uint32_t Image
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.LevelCrossing;
		public static int StructSize => 0x12;
	}
}
