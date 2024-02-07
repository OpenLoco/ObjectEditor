using System.ComponentModel;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;

namespace OpenLocoObjectEditor.Objects
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
	public record TreeObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t Clearance,
		[property: LocoStructOffset(0x03)] uint8_t Height,
		[property: LocoStructOffset(0x04)] uint8_t var_04,
		[property: LocoStructOffset(0x05)] uint8_t var_05,
		[property: LocoStructOffset(0x06)] uint8_t NumRotations,
		[property: LocoStructOffset(0x07)] uint8_t Growth,
		[property: LocoStructOffset(0x08)] TreeObjectFlags Flags,
		[property: LocoStructOffset(0x0A), LocoArrayLength(6), Browsable(false)] image_id[] Sprites,
		[property: LocoStructOffset(0x22), LocoArrayLength(6), Browsable(false)] image_id[] SnowSprites,
		[property: LocoStructOffset(0x3A)] uint16_t ShadowImageOffset,
		[property: LocoStructOffset(0x3C)] uint8_t var_3C,
		[property: LocoStructOffset(0x3D)] uint8_t SeasonState,
		[property: LocoStructOffset(0x3E)] uint8_t var_3E,
		[property: LocoStructOffset(0x3F)] uint8_t CostIndex,
		[property: LocoStructOffset(0x40)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x42)] int16_t ClearCostFactor,
		[property: LocoStructOffset(0x44)] uint32_t Colours,
		[property: LocoStructOffset(0x48)] int16_t Rating,
		[property: LocoStructOffset(0x4A)] int16_t DemolishRatingReduction
	) : ILocoStruct;
}
