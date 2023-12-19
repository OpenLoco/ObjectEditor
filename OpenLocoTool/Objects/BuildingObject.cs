using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum BuildingObjectFlags : uint8_t
	{
		None = 0,
		LargeTile = 1 << 0, // 2x2 tile
		MiscBuilding = 1 << 1,
		Indestructible = 1 << 2,
		IsHeadquarters = 1 << 3,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xBE)]
	//[LocoStringTable("Name")]
	public record BuildingObject(
			[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
			[property: LocoStructOffset(0x02)] uint32_t Image,
			[property: LocoStructOffset(0x06)] uint8_t var_06,
			[property: LocoStructOffset(0x07)] uint8_t NumVariations,
			[property: LocoStructOffset(0x08), LocoArrayLength(4)] uint8_t[] VariationHeights,
			[property: LocoStructOffset(0x0C), LocoArrayLength(2)] uint16_t[] var_0C,
			//[property: LocoStructProperty(0x10)] uint8_t* variationsArr10[32],
			[property: LocoStructOffset(0x90)] uint32_t Colours,
			[property: LocoStructOffset(0x94)] uint16_t DesignedYear,
			[property: LocoStructOffset(0x96)] uint16_t ObsoleteYear,
			[property: LocoStructOffset(0x98)] BuildingObjectFlags Flags,
			[property: LocoStructOffset(0x99)] uint8_t ClearCostIndex,
			[property: LocoStructOffset(0x9A)] uint16_t ClearCostFactor,
			[property: LocoStructOffset(0x9C)] uint8_t ScaffoldingSegmentType,
			[property: LocoStructOffset(0x9D)] Colour ScaffoldingColour,
			[property: LocoStructOffset(0x9E), LocoArrayLength(0xA0 - 0x9E)] uint8_t[] pad_9E,
			[property: LocoStructOffset(0xA0), LocoArrayLength(2)] uint8_t[] ProducedQuantity,
			[property: LocoStructOffset(0xA2), LocoArrayLength(2)] uint8_t[] ProducedCargoType,
			[property: LocoStructOffset(0xA6), LocoArrayLength(2)] uint8_t[] var_A6,
			[property: LocoStructOffset(0xA8), LocoArrayLength(2)] uint8_t[] var_A8,
			[property: LocoStructOffset(0xA4), LocoArrayLength(2)] uint8_t[] var_A4, // Some type of Cargo
			[property: LocoStructOffset(0xAA)] int16_t DemolishRatingReduction,
			[property: LocoStructOffset(0xAC)] uint8_t var_AC,
			[property: LocoStructOffset(0xAD)] uint8_t var_AD
		//[property: LocoStructProperty(0xAE)] const uint8_t* var_AE[4] // 0xAE ->0xB2->0xB6->0xBA->0xBE (4 byte pointers)
		) : ILocoStruct, ILocoStructVariableData
	{
		public static ObjectType ObjectType => ObjectType.Building;
		public static int StructSize => 0xBE;

		// known issues:
		// HOSPITL1.dat - loads without error but graphics are bugged
		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// variationHeights
			remainingData = remainingData[(var_06 * 1)..]; // sizeof(uint8_t)

			// part animations
			remainingData = remainingData[(var_06 * 2)..]; // sizeof(uint16_t)

			// parts
			for (var i = 0; i < NumVariations; ++i)
			{
				var ptr_vari = 0;
				while (remainingData[ptr_vari] != 0xFF)
				{
					ptr_vari++;
				}

				ptr_vari++;
				remainingData = remainingData[ptr_vari..];
			}

			// produced cargo
			remainingData = remainingData[(S5Header.StructLength * ProducedCargoType.Length)..];

			// load ??? cargo (probably required cargo as this matches IndustryObject)
			remainingData = remainingData[(S5Header.StructLength * var_A4.Length)..];

			// load ??
			var ptr_AD = 0;
			for (var i = 0; i < var_AD; ++i)
			{
				var size = BitConverter.ToUInt16(remainingData[..2]);
				remainingData = remainingData[(2 + size)..];
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> Save() => throw new NotImplementedException();
	}
}
