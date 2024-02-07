using System.ComponentModel;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;

namespace OpenLocoObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x38)]
	[LocoStructType(ObjectType.Competitor)]
	[LocoStringTable("Full Name", "Last Name")]
	public record CompetitorObject(
			[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id FullName,
			[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id LastName,
			[property: LocoStructOffset(0x04)] uint32_t var_04,
			[property: LocoStructOffset(0x08)] uint32_t var_08,
			[property: LocoStructOffset(0x0C)] uint8_t Emotions,
			[property: LocoStructOffset(0x10), Browsable(false), LocoArrayLength(CompetitorObject.ImagesLength)] image_id[] Images,
			[property: LocoStructOffset(0x34)] uint8_t Intelligence,
			[property: LocoStructOffset(0x35)] uint8_t Aggressiveness,
			[property: LocoStructOffset(0x36)] uint8_t Competitiveness,
			[property: LocoStructOffset(0x37)] uint8_t var_37
		) : ILocoStruct
	{
		public const int ImagesLength = 9;
	}
}
