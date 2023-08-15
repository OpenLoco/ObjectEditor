
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	public record RoadExtraObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint16_t RoadPieces,
		[property: LocoStructProperty(0x04)] uint8_t PaintStyle,
		[property: LocoStructProperty(0x05)] uint8_t CostIndex,
		[property: LocoStructProperty(0x06)] int16_t BuildCostFactor,
		[property: LocoStructProperty(0x08)] int16_t SellCostFactor,
		[property: LocoStructProperty(0x0A)] uint32_t Image,
		[property: LocoStructProperty(0x0E)] uint32_t var_0E
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.roadExtra;
		public static int StructLength => 0x12;
	}
}
