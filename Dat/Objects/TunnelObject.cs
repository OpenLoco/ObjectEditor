
using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	[LocoStructType(ObjectType.Tunnel)]
	[LocoStringTable("Name")]
	public record TunnelObject(
		[property: LocoStructOffset(0x00), Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] image_id Image
		) : ILocoStruct
	{
		public bool Validate() => true;
	}
}
