using System.ComponentModel;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;

namespace OpenLocoObjectEditor.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStructType(ObjectType.RoadExtra)]
	[LocoStringTable("Name")]
	public class RoadExtraObject(
		uint16_t roadPieces,
		uint8_t paintStyle,
		uint8_t costIndex,
		int16_t buildCostFactor,
		int16_t sellCostFactor)
		: ILocoStruct
	{
		//[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[LocoStructOffset(0x02)] public uint16_t RoadPieces { get; set; } = roadPieces;
		[LocoStructOffset(0x04)] public uint8_t PaintStyle { get; set; } = paintStyle;
		[LocoStructOffset(0x05)] public uint8_t CostIndex { get; set; } = costIndex;
		[LocoStructOffset(0x06)] public int16_t BuildCostFactor { get; set; } = buildCostFactor;
		[LocoStructOffset(0x08)] public int16_t SellCostFactor { get; set; } = sellCostFactor;
		//[LocoStructOffset(0x0A)] public image_id Image { get; set; }
		//[LocoStructOffset(0x0E)] public image_id var_0E { get; set; }
	}
}
