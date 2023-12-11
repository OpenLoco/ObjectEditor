
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	//[LocoStringTable("Name")]
	public record TunnelObject(
		//[property: LocoStructOffset(0x00)] string_id Name,
		//[property: LocoStructOffset(0x02)] uint32_t Image
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.Tunnel;
		public static int StructSize => 0x06;
	}
}
