using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace OpenLoco.ObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStructType(ObjectType.Scaffolding)]
	[LocoStringTable("Name")]
	public record ScaffoldingObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x06), LocoArrayLength(3)] uint16_t[] SegmentHeights,
		[property: LocoStructOffset(0x0C), LocoArrayLength(3)] uint16_t[] RoofHeights
		) : ILocoStruct
	{
		public bool Validate() => true;
	}

}
