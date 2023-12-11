using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x38)]
	//[LocoStringTable("var_00, var_02")]
	public record CompetitorObject(
		//[property: LocoStructOffset(0x00)] string_id var_00,
		//[property: LocoStructOffset(0x02)] string_id var_02,
		[property: LocoStructOffset(0x04)] uint32_t var_04,
		[property: LocoStructOffset(0x08)] uint32_t var_08,
		[property: LocoStructOffset(0x0C)] uint32_t Emotions,
		[property: LocoStructOffset(0x10), LocoArrayLength(CompetitorObject.ImagesLength)] uint32_t[] Images,
		[property: LocoStructOffset(0x34)] uint8_t Intelligence,
		[property: LocoStructOffset(0x35)] uint8_t Aggressiveness,
		[property: LocoStructOffset(0x36)] uint8_t Competitiveness,
		[property: LocoStructOffset(0x37)] uint8_t var_37
		) : ILocoStruct
	{
		public static ObjectType ObjectType => ObjectType.Competitor;

		public static int StructSize => 0x38;

		public const int ImagesLength = 9;
	}
}
