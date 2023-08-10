using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record HillShapesObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint8_t HillHeightMapCount,
		[property: LocoStructProperty(0x03)] uint8_t MountainHeightMapCount,
		[property: LocoStructProperty(0x04)] uint32_t Image,
		[property: LocoStructProperty(0x08)] uint32_t var_08,
		[property: LocoStructProperty(0x0C), LocoArrayLength(0x0E - 0x0C)] uint8_t[] pad_0C
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.hillShapes;
		public static int StructLength => 0xE;
	}
}
