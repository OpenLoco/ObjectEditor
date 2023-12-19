
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	public record ScenarioTextObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), LocoString, Browsable(false)] string_id Details,
		[property: LocoStructOffset(0x04), LocoArrayLength(0x6 - 0x4), LocoString, Browsable(false)] string_id pad_04 // 0x04
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.ScenarioText;
		public static int StructSize => 0x06;
	}
}
