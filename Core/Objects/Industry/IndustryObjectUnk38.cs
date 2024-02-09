using System.ComponentModel;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x02)]

	public record IndustryObjectUnk38(
		[property: LocoStructOffset(0x00)] uint8_t var_00,
		[property: LocoStructOffset(0x01)] uint8_t var_01
		) : ILocoStruct;
}
