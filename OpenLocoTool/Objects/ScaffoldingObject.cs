
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	public record ScaffoldingObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] uint32_t Image,
		[property: LocoStructOffset(0x06), LocoArrayLength(3)] uint16_t[] SegmentHeights, // 0x06
		[property: LocoStructOffset(0x0C), LocoArrayLength(3)] uint16_t[] RoofHeights    // 0x0C
	) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.Scaffolding;
		public static int StructSize => 0x12;
	}
}
