using System.ComponentModel;
using OpenLocoTool.DatFileParsing;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	public record BogieSprite(
		[property: LocoStructOffset(0x00)] uint8_t RollStates,      // valid values 1, 2, 4 related to bogie->var_46 (identical in value to numRollSprites)
		[property: LocoStructOffset(0x01)] BogieSpriteFlags Flags,  // BogieSpriteFlags
		[property: LocoStructOffset(0x02)] uint8_t Width,           // sprite width
		[property: LocoStructOffset(0x03)] uint8_t HeightNegative,  // sprite height negative
		[property: LocoStructOffset(0x04)] uint8_t HeightPositive,  // sprite height positive
		[property: LocoStructOffset(0x05)] uint8_t NumRollSprites,
		[property: LocoStructOffset(0x06)] uint32_t FlatImageIds,   // flat sprites
		[property: LocoStructOffset(0x0A)] uint32_t GentleImageIds, // gentle sprites
		[property: LocoStructOffset(0x0E)] uint32_t SteepImageIds   // steep sprites
		) : ILocoStruct;
}
