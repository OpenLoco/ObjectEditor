
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	[LocoStringCount(2)]
	public record ScenarioTextObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] string_id Details,
		[property: LocoStructOffset(0x04), LocoArrayLength(0x6 - 0x4)] string_id pad_04 // 0x04
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.scenarioText;
		public static int StructSize => 0x06;
	}
}
