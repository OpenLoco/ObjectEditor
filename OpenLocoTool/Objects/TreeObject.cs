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

	public record TreeObject(
		[property: LocoStructProperty] string_id Name,                  // 0x00
		[property: LocoStructProperty] uint8_t var_02,                  // 0x02
		[property: LocoStructProperty] uint8_t Height,                  // 0x03
		[property: LocoStructProperty] uint8_t var_04,                  // 0x04
		[property: LocoStructProperty] uint8_t var_05,                  // 0x05
		[property: LocoStructProperty] uint8_t NumRotations,            // 0x06 (1,2,4)
		[property: LocoStructProperty] uint8_t Growth,                  // 0x07 (number of tree size images)
		[property: LocoStructProperty] TreeObjectFlags Flags,           // 0x08
		[property: LocoStructProperty, LocoArrayLength(6)] uint32_t[] Sprites,             // 0x0A
		[property: LocoStructProperty, LocoArrayLength(6)] uint32_t[] SnowSprites,         // 0x22
		[property: LocoStructProperty] uint16_t ShadowImageOffset,      // 0x3A
		[property: LocoStructProperty] uint8_t var_3C,                  // 0x3C
		[property: LocoStructProperty] uint8_t SeasonState,             // 0x3D (index for sprites, seasons + dying)
		[property: LocoStructProperty] uint8_t var_3E,                  // 0x3E
		[property: LocoStructProperty] uint8_t CostIndex,               // 0x3F
		[property: LocoStructProperty] int16_t BuildCostFactor,         // 0x40
		[property: LocoStructProperty] int16_t ClearCostFactor,         // 0x42
		[property: LocoStructProperty] uint32_t Colours,                // 0x44
		[property: LocoStructProperty] int16_t Rating,                  // 0x48
		[property: LocoStructProperty] int16_t DemolishRatingReduction // 0x4A
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.tree;
		public int ObjectStructSize => 0x4C;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
