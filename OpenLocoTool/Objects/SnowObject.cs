
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	[LocoStructType(ObjectType.Snow)]
	[LocoStringTable("Name")]
	public class SnowObject : ILocoStruct
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		//[LocoStructOffset(0x02)] image_id Image
	}
}
