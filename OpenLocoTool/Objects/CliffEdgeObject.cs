using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	public record CliffEdgeObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint32_t Image
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.cliffEdge;

		public static int StructLength => 0x06;
	}
}
