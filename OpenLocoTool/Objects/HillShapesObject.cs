using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0E)]
	[LocoStructType(ObjectType.HillShapes)]
	[LocoStringTable("Name")]
	public class HillShapesObject(
		uint8_t hillHeightMapCount,
		uint8_t mountainHeightMapCount)
		: ILocoStruct
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name { get; set; }
		[LocoStructOffset(0x02)] public uint8_t HillHeightMapCount { get; set; } = hillHeightMapCount;
		[LocoStructOffset(0x03)] public uint8_t MountainHeightMapCount { get; set; } = mountainHeightMapCount;
		//[LocoStructOffset(0x04)] image_id Image { get; set; }
		//[LocoStructOffset(0x08)] image_id ImageHill { get; set; }
		//[LocoStructOffset(0x0C), LocoArrayLength(0x0E - 0x0C)] uint8_t[] pad_0C { get; set; }
	}
}
