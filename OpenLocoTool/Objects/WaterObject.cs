
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record WaterObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint8_t CostIndex,
		[property: LocoStructProperty(0x03)] uint8_t var_03,
		[property: LocoStructProperty(0x04)] int8_t CostFactor,
		[property: LocoStructProperty(0x05)] uint8_t var_05,
		[property: LocoStructProperty(0x06)] uint32_t Image,
		[property: LocoStructProperty(0x0A)] uint32_t var_0A
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.water;
		public static int StructLength => 0xE;
	}
}
