using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	[LocoStructType(ObjectType.CliffEdge)]
	public class CliffEdgeObject(string_id name, uint32_t image) : ILocoStruct
	{
		[LocoStructOffset(0x00), LocoString, Browsable(false)] public string_id Name { get; set; } = name;
		[LocoStructOffset(0x02)] public uint32_t Image { get; set; } = image;
	}
}
