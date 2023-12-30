
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStructType(ObjectType.Scaffolding)]
	[LocoStringTable("Name")]
	public class ScaffoldingObject(
		uint16_t[] segmentHeights,
		uint16_t[] roofHeights)
		: ILocoStruct
	{
		//[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		//[property: LocoStructOffset(0x02)] image_id Image,
		[LocoStructOffset(0x06), LocoArrayLength(3)] public uint16_t[] SegmentHeights { get; set; } = segmentHeights;
		[LocoStructOffset(0x0C), LocoArrayLength(3)] public uint16_t[] RoofHeights { get; set; } = roofHeights;
	}
}
