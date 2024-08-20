using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x02)]

	public record IndustryObjectUnk38(
		[property: LocoStructOffset(0x00)] uint8_t var_00,
		[property: LocoStructOffset(0x01)] uint8_t var_01
		) : ILocoStruct
	{
		public bool Validate() => true;
	}
}
