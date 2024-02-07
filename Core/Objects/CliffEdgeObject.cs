using System.ComponentModel;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;

namespace OpenLocoObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	[LocoStructType(ObjectType.CliffEdge)]
	[LocoStringTable("Name")]
	public record CliffEdgeObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name
		) : ILocoStruct;
}
