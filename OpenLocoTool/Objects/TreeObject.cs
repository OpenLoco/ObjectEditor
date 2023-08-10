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
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint8_t var_02,
		[property: LocoStructProperty(0x03)] uint8_t Height,
		[property: LocoStructProperty(0x04)] uint8_t var_04,
		[property: LocoStructProperty(0x05)] uint8_t var_05,
		[property: LocoStructProperty(0x06)] uint8_t NumRotations, //  (1,2,4)
		[property: LocoStructProperty(0x07)] uint8_t Growth, // (number of tree size images)
		[property: LocoStructProperty(0x08)] TreeObjectFlags Flags,
		[property: LocoStructProperty(0x0A), LocoArrayLength(6)] uint32_t[] Sprites,             // 0x0A
		[property: LocoStructProperty(0x22), LocoArrayLength(6)] uint32_t[] SnowSprites,         // 0x22
		[property: LocoStructProperty(0x3A)] uint16_t ShadowImageOffset,
		[property: LocoStructProperty(0x3C)] uint8_t var_3C,
		[property: LocoStructProperty(0x3D)] uint8_t SeasonState, // (index for sprites, seasons + dying)
		[property: LocoStructProperty(0x3E)] uint8_t var_3E,
		[property: LocoStructProperty(0x3F)] uint8_t CostIndex,
		[property: LocoStructProperty(0x40)] int16_t BuildCostFactor,
		[property: LocoStructProperty(0x42)] int16_t ClearCostFactor,
		[property: LocoStructProperty(0x44)] uint32_t Colours,
		[property: LocoStructProperty(0x48)] int16_t Rating,
		[property: LocoStructProperty(0x4A)] int16_t DemolishRatingReduction
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.tree;
		public static int StructLength => 0x4C;
	}
}
