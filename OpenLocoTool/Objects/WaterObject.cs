
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x0E)]
	[LocoStructType(ObjectType.Water)]
	[LocoStringTable("Name")]
	public class WaterObject(
		uint8_t costIndex,
		uint8_t var_03,
		int8_t costFactor,
		uint8_t var_05)
		: ILocoStruct
	{

		//[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[LocoStructOffset(0x02)] public uint8_t CostIndex { get; set; } = costIndex;
		[LocoStructOffset(0x03), LocoPropertyMaybeUnused] public uint8_t var_03 { get; set; } = var_03;
		[LocoStructOffset(0x04)] public int8_t CostFactor { get; set; } = costFactor;
		[LocoStructOffset(0x05), LocoPropertyMaybeUnused] public uint8_t var_05 { get; set; } = var_05;
		//[LocoStructOffset(0x06)] public image_id Image,
		//[LocoStructOffset(0x0A)] public image_id MapPixelImage { get; set; }
	}
}
