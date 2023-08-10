
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record ScenarioTextObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] string_id Details,
		[property: LocoStructProperty(0x04), LocoArrayLength(0x6 - 0x4)] string_id pad_04 // 0x04
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.scenarioText;
		public static int ObjectStructSize => 0x6;
	}
}
