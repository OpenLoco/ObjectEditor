
using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace OpenLoco.ObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	[LocoStructType(ObjectType.ScenarioText)]
	[LocoStringTable("Name", "Details")]
	public record ScenarioTextObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), LocoString, Browsable(false)] string_id Details,
		[property: LocoStructOffset(0x04), LocoArrayLength(0x6 - 0x4)] uint8_t pad_04
		) : ILocoStruct
	{
		public bool Validate() => throw new NotImplementedException();
	}
}
