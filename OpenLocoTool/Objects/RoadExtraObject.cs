
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStringCount(1)]
	public record RoadExtraObject(
		//[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint16_t RoadPieces,
		[property: LocoStructOffset(0x04)] uint8_t PaintStyle,
		[property: LocoStructOffset(0x05)] uint8_t CostIndex,
		[property: LocoStructOffset(0x06)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
		//[property: LocoStructOffset(0x0A)] uint32_t Image,
		[property: LocoStructOffset(0x0E)] uint32_t var_0E
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.RoadExtra;
		public static int StructSize => 0x12;
	}
}
