
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0E)]
	public record WaterObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t CostIndex,
		[property: LocoStructOffset(0x03)] uint8_t var_03,
		[property: LocoStructOffset(0x04)] int8_t CostFactor,
		[property: LocoStructOffset(0x05)] uint8_t var_05,
		[property: LocoStructOffset(0x06)] uint32_t Image,
		[property: LocoStructOffset(0x0A)] uint32_t var_0A
	) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.water;
		public static int StructLength => 0x0E;
	}
}
