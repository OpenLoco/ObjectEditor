
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	[LocoStructType(ObjectType.LevelCrossing)]
	[LocoStringTable("Name")]
	public class LevelCrossingObject(
		int16_t costFactor,
		int16_t sellCostFactor,
		uint8_t costIndex,
		uint8_t animationSpeed,
		uint8_t closingFrames,
		uint8_t closedFrames,
		uint16_t designedYear) : ILocoStruct
	{
		//[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[LocoStructOffset(0x02)] public int16_t CostFactor { get; set; } = costFactor;
		[LocoStructOffset(0x04)] public int16_t SellCostFactor { get; set; } = sellCostFactor;
		[LocoStructOffset(0x06)] public uint8_t CostIndex { get; set; } = costIndex;
		[LocoStructOffset(0x07)] public uint8_t AnimationSpeed { get; set; } = animationSpeed;
		[LocoStructOffset(0x08)] public uint8_t ClosingFrames { get; set; } = closingFrames;
		[LocoStructOffset(0x09)] public uint8_t ClosedFrames { get; set; } = closedFrames;
		//[LocoStructOffset(0x0A), LocoArrayLength(0x0C - 0x0A)] public uint8_t[] pad_0A
		[LocoStructOffset(0x0C)] public uint16_t DesignedYear { get; set; } = designedYear;
		//[property: LocoStructOffset(0x0E)] image_id Image
	}
}
