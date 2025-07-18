using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using System.ComponentModel;

namespace Dat.Objects;

[Flags]
public enum TreeObjectFlags : uint16_t
{
	None = 0,
	HasSnowVariation = 1 << 0,
	unk_01 = 1 << 1,
	HighAltitude = 1 << 2,
	LowAltitude = 1 << 3,
	RequiresWater = 1 << 4,
	unk_05 = 1 << 5,
	DroughtResistant = 1 << 6,
	HasShadow = 1 << 7,
}

[Flags]
public enum UnkTreeFlags : uint8_t
{
	unk_00 = 1 << 0,
	unk_01 = 1 << 1,
	unk_02 = 1 << 2,
	unk_03 = 1 << 3,
	unk_04 = 1 << 4,
	unk_05 = 1 << 5,
}

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
	[property: LocoStructOffset(0x07)] uint8_t NumGrowthStages,
	[property: LocoStructOffset(0x08)] TreeObjectFlags Flags,
	[property: LocoStructOffset(0x0A), LocoArrayLength(6), Browsable(false)] image_id[] Sprites,
	[property: LocoStructOffset(0x22), LocoArrayLength(6), Browsable(false)] image_id[] SnowSprites,
	[property: LocoStructOffset(0x3A), Browsable(false)] uint16_t ShadowImageOffset,
	[property: LocoStructOffset(0x3C)] UnkTreeFlags var_3C, // something with images
	[property: LocoStructOffset(0x3D)] uint8_t SeasonState,
	[property: LocoStructOffset(0x3E)] uint8_t Season,
	[property: LocoStructOffset(0x3F)] uint8_t CostIndex,
	[property: LocoStructOffset(0x40)] int16_t BuildCostFactor,
	[property: LocoStructOffset(0x42)] int16_t ClearCostFactor,
	[property: LocoStructOffset(0x44)] uint32_t Colours,
	[property: LocoStructOffset(0x48)] int16_t Rating,
	[property: LocoStructOffset(0x4A)] int16_t DemolishRatingReduction
	) : ILocoStruct
{
	public bool Validate()
	{
		if (CostIndex > 32)
		{
			return false;
		}

		// 230/256 = ~90%
		if (-ClearCostFactor > BuildCostFactor * 230 / 256)
		{
			return false;
		}

		switch (NumRotations)
		{
			default:
				return false;
			case 1:
			case 2:
			case 4:
				break;
		}

		if (NumGrowthStages is < 1 or > 8)
		{
			return false;
		}

		if (Height < Clearance)
		{
			return false;
		}

		return var_05 >= var_04;
	}
}
