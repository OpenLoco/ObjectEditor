
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	public record RegionObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint32_t Image,
		[property: LocoStructProperty(0x06), LocoArrayLength(0x8 - 0x6)] uint8_t[] pad_06,
		[property: LocoStructProperty(0x08)] uint8_t var_08,
		[property: LocoStructProperty(0x09), LocoArrayLength(4)] uint8_t[] var_09,
		[property: LocoStructProperty(0x0D), LocoArrayLength(0x12 - 0xD)] uint8_t[] pad_0D
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.region;
		public static int StructLength => 0x12;
	}
}
