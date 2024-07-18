using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace OpenLoco.ObjectEditor.Objects
{
	[Flags]
	public enum HillShapeFlags : uint16_t
	{
		None = 0,
		IsHeightMap = 1 << 0,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0E)]
	[LocoStructType(ObjectType.HillShapes)]
	[LocoStringTable("Name")]
	public record HillShapesObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t HillHeightMapCount,
		[property: LocoStructOffset(0x03)] uint8_t MountainHeightMapCount,
		[property: LocoStructOffset(0x04), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x08), Browsable(false)] image_id ImageHill,
		[property: LocoStructOffset(0x0C)] HillShapeFlags Flags
		) : ILocoStruct
	{
		public bool Validate() => true;
	}
}
