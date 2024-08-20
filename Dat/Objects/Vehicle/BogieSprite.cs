using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;
using System.ComponentModel;

namespace OpenLoco.Dat.Objects
{
	public enum BogieSpriteSlopeType
	{
		Flat,
		Gentle,
		Steep
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x12)]
	public record BogieSprite(
		[property: LocoStructOffset(0x00)] uint8_t RollStates,      // valid values 1, 2, 4 related to bogie->var_46 (identical in value to numRollSprites)
		[property: LocoStructOffset(0x01)] BogieSpriteFlags Flags,  // BogieSpriteFlags
		[property: LocoStructOffset(0x02)] uint8_t Width,           // sprite width
		[property: LocoStructOffset(0x03)] uint8_t HeightNegative,  // sprite height negative
		[property: LocoStructOffset(0x04)] uint8_t HeightPositive,  // sprite height positive
		[property: LocoStructOffset(0x05), LocoStructVariableLoad, Browsable(false)] uint8_t _NumRollSprites,
		[property: LocoStructOffset(0x06), LocoStructVariableLoad, Browsable(false)] uint32_t _FlatImageIds,   // flat sprites
		[property: LocoStructOffset(0x0A), LocoStructVariableLoad, Browsable(false)] uint32_t _GentleImageIds, // gentle sprites
		[property: LocoStructOffset(0x0E), LocoStructVariableLoad, Browsable(false)] uint32_t _SteepImageIds   // steep sprites
		) : ILocoStruct
	{
		public uint8_t NumRollSprites { get; set; }

		public Dictionary<BogieSpriteSlopeType, List<int>> ImageIds = [];
		public int NumImages { get; set; }

		public bool Validate() => true;
	}
}
