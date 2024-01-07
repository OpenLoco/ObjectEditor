
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	[LocoStructType(ObjectType.Tunnel)]
	[LocoStringTable("Name")]
	public class TunnelObject : ILocoStruct
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		//[LocoStructOffset(0x02)] public image_id Image
	}
}
