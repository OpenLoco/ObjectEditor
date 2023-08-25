using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Headers
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[Category("Header")]
	[LocoStructSize(0x06)]
	public record StringTableResult(
		[property: LocoStructOffset(0x00)] string_id Str,
		[property: LocoStructOffset(0x04)] uint32_t TableLength);
}
