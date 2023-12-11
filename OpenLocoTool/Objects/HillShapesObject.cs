using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0E)]
	//[LocoStringTable("Name")]
	public record HillShapesObject(
		//[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t HillHeightMapCount,
		[property: LocoStructOffset(0x03)] uint8_t MountainHeightMapCount,
		//[property: LocoStructOffset(0x04)] uint32_t Image,
		[property: LocoStructOffset(0x08)] uint32_t var_08,
		[property: LocoStructOffset(0x0C), LocoArrayLength(0x0E - 0x0C)] uint8_t[] pad_0C
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.HillShapes;
		public static int StructSize => 0x0E;
	}
}
