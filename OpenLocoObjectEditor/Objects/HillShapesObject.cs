using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0E)]
	[LocoStructType(ObjectType.HillShapes)]
	[LocoStringTable("Name")]
	public record HillShapesObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t HillHeightMapCount,
		[property: LocoStructOffset(0x03)] uint8_t MountainHeightMapCount,
		[property: LocoStructOffset(0x04), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x08), Browsable(false)] image_id ImageHill,
		[property: LocoStructOffset(0x0C), LocoArrayLength(0x0E - 0x0C), Browsable(false)] uint8_t[] pad_0C
	) : ILocoStruct;
}
