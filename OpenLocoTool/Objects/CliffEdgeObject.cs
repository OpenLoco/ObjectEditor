using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x06)]
	//[LocoStringTable("Name")]
	public class CliffEdgeObject : ILocoStruct
	{
		public const ObjectType ObjType = ObjectType.CliffEdge;
		public const int StructSize = 0x06;

		public CliffEdgeObject(/*string_id name, uint32_t image*/)
		{
			//Name = name;
			//Image = image;
		}

		//[LocoStructOffset(0x00)] public string_id Name { get; set; }
		//[LocoStructOffset(0x02)] public uint32_t Image { get; set; }
	}
}
