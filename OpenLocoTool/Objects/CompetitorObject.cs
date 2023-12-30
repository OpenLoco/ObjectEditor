using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x38)]
	[LocoStructType(ObjectType.Competitor)]
	[LocoStringTable("Full Name", "Last Name")]
	public class CompetitorObject(
		uint32_t var_04,
		uint32_t var_08,
		uint32_t emotions,
		uint8_t intelligence,
		uint8_t aggressiveness,
		uint8_t competitiveness,
		uint8_t var_37)
		: ILocoStruct
	{
		//[LocoStructOffset(0x00), LocoString] public string_id var_00 { get; set; }
		//[LocoStructOffset(0x02), LocoString, Browsable(false)] public string_id var_02 { get; set; }
		[LocoStructOffset(0x04)] public uint32_t var_04 { get; set; } = var_04;
		[LocoStructOffset(0x08)] public uint32_t var_08 { get; set; } = var_08;
		[LocoStructOffset(0x0C)] public uint32_t Emotions { get; set; } = emotions;
		//[LocoStructOffset(0x10), LocoArrayLength(ImagesLength)] public image_id[] Images { get; set; } = new uint32_t[ImagesLength];
		[LocoStructOffset(0x34)] public uint8_t Intelligence { get; set; } = intelligence;
		[LocoStructOffset(0x35)] public uint8_t Aggressiveness { get; set; } = aggressiveness;
		[LocoStructOffset(0x36)] public uint8_t Competitiveness { get; set; } = competitiveness;
		[LocoStructOffset(0x37)] public uint8_t var_37 { get; set; } = var_37;

		//public const int ImagesLength = 9;
	}
}
