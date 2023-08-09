
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record ScenarioTextObject(
		[property: LocoStructProperty] string_id Name,                              // 0x00
		[property: LocoStructProperty] string_id Details,                           // 0x02
		[property: LocoStructProperty, LocoArrayLength(0x6 - 0x4)] string_id pad_04 // 0x04
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.scenarioText;
		public int ObjectStructSize => 0x6;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
