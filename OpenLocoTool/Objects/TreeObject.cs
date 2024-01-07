using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum TreeObjectFlags : uint16_t
	{
		None = 0,
		HasSnowVariation = 1 << 0,
		unk1 = 1 << 1,
		VeryHighAltitude = 1 << 2,
		HighAltitude = 1 << 3,
		RequiresWater = 1 << 4,
		unk5 = 1 << 5,
		DroughtResistant = 1 << 6,
		HasShadow = 1 << 7,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x4C)]
	[LocoStructType(ObjectType.Tree)]
	[LocoStringTable("Name")]
	public class TreeObject(
		uint8_t initialHeight,
		uint8_t maxHeight,
		uint8_t var_04,
		uint8_t var_05,
		uint8_t numRotations,
		uint8_t growth,
		TreeObjectFlags flags,
		uint8_t var_3C,
		uint8_t seasonState,
		uint8_t var_3E,
		uint8_t costIndex,
		int16_t buildCostFactor,
		int16_t clearCostFactor,
		uint32_t colours,
		int16_t rating,
		int16_t demolishRatingReduction)
		: ILocoStruct
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[LocoStructOffset(0x02)] public uint8_t Clearance { get; set; } = initialHeight;
		[LocoStructOffset(0x03)] public uint8_t Height { get; set; } = maxHeight;
		[LocoStructOffset(0x04)] public uint8_t var_04 { get; set; } = var_04;
		[LocoStructOffset(0x05)] public uint8_t var_05 { get; set; } = var_05;
		[LocoStructOffset(0x06)] public uint8_t NumRotations { get; set; } = numRotations;
		[LocoStructOffset(0x07)] public uint8_t Growth { get; set; } = growth;
		[LocoStructOffset(0x08)] public TreeObjectFlags Flags { get; set; } = flags;
		//[LocoStructOffset(0x0A), LocoArrayLength(6)] public image_id[] Sprites { get; set; } = sprites;
		//[LocoStructOffset(0x22), LocoArrayLength(6)] public image_id[] SnowSprites { get; set; } = snowSprites;
		//[LocoStructOffset(0x3A)] public uint16_t ShadowImageOffset { get; set; } = shadowImageOffset;
		[LocoStructOffset(0x3C)] public uint8_t var_3C { get; set; } = var_3C;
		[LocoStructOffset(0x3D)] public uint8_t SeasonState { get; set; } = seasonState;
		[LocoStructOffset(0x3E)] public uint8_t var_3E { get; set; } = var_3E; // something to do with season
		[LocoStructOffset(0x3F)] public uint8_t CostIndex { get; set; } = costIndex;
		[LocoStructOffset(0x40)] public int16_t BuildCostFactor { get; set; } = buildCostFactor;
		[LocoStructOffset(0x42)] public int16_t ClearCostFactor { get; set; } = clearCostFactor;
		[LocoStructOffset(0x44)] public uint32_t Colours { get; set; } = colours;
		[LocoStructOffset(0x48)] public int16_t Rating { get; set; } = rating;
		[LocoStructOffset(0x4A)] public int16_t DemolishRatingReduction { get; set; } = demolishRatingReduction;
	}
}
