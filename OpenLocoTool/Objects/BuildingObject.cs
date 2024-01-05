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
	[LocoStructType(ObjectType.Building)]
	[LocoStringTable("Name")]
	public record BuildingObject(
			//[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
			//[property: LocoStructOffset(0x02)] image_id Image,
			[property: LocoStructOffset(0x06)] uint8_t NumSpriteSets,
			[property: LocoStructOffset(0x07)] uint8_t NumParts,
			[property: LocoStructOffset(0x08), LocoStructVariableLoad, LocoArrayLength(4)] List<uint8_t> PartHeights,
			[property: LocoStructOffset(0x0C), LocoStructVariableLoad, LocoArrayLength(2)] List<uint16_t> PartAnimations,
			[property: LocoStructOffset(0x10), LocoStructVariableLoad, LocoArrayLength(32)] List<uint8_t[]> Parts,
			[property: LocoStructOffset(0x90)] uint32_t Colours,
			[property: LocoStructOffset(0x94)] uint16_t DesignedYear,
			[property: LocoStructOffset(0x96)] uint16_t ObsoleteYear,
			[property: LocoStructOffset(0x98)] BuildingObjectFlags Flags,
			[property: LocoStructOffset(0x99)] uint8_t ClearCostIndex,
			[property: LocoStructOffset(0x9A)] uint16_t ClearCostFactor,
			[property: LocoStructOffset(0x9C)] uint8_t ScaffoldingSegmentType,
			[property: LocoStructOffset(0x9D)] Colour ScaffoldingColour,
			[property: LocoStructOffset(0x9E)] uint8_t GeneratorFunction,
			[property: LocoStructOffset(0x9F)] uint8_t var_9F,
			//[property: LocoStructOffset(0xA0), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> ProducedQuantity,
			//[property: LocoStructOffset(0xA2), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> ProducedCargoType,
			//[property: LocoStructOffset(0xA4), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> RequiredCargoType,
			[property: LocoStructOffset(0xA6), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> var_A6,
			[property: LocoStructOffset(0xA8), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> var_A8,
			[property: LocoStructOffset(0xA4), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> var_A4, // Some type of Cargo
			[property: LocoStructOffset(0xAA)] int16_t DemolishRatingReduction,
			[property: LocoStructOffset(0xAC)] uint8_t var_AC,
			[property: LocoStructOffset(0xAD)] uint8_t var_AD
		//[property: LocoStructOffset(0xAE), LocoStructVariableLoad, LocoArrayLength(4)] uint8_t[][] var_AE // 0xAE ->0xB2->0xB6->0xBA->0xBE (4 byte pointers)
		) : ILocoStruct, ILocoStructVariableData
	{
		public List<S5Header> ProducedCargo { get; set; } = [];
		public List<S5Header> RequiredCargo { get; set; } = [];

		public List<uint8_t[]> var_AE { get; set; } = [];

		// known issues:
		// HOSPITL1.dat - loads without error but graphics are bugged
		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// partHeights
			PartHeights.Clear();
			PartHeights.AddRange(ByteReaderT.Read_Array<uint8_t>(remainingData[..(NumSpriteSets * 1)], NumSpriteSets));
			remainingData = remainingData[(NumSpriteSets * 1)..]; // uint8_t*

			// part animations
			PartAnimations.Clear();
			PartAnimations.AddRange(ByteReaderT.Read_Array<uint16_t>(remainingData[..(NumSpriteSets * 2)], NumSpriteSets));
			remainingData = remainingData[(NumSpriteSets * 2)..]; // uint16_t*

			// parts
			Parts.Clear();
			for (var i = 0; i < NumParts; ++i)
			{
				var ptr_vari = 0;
				while (remainingData[++ptr_vari] != 0xFF) ;
				Parts.Add(remainingData[..ptr_vari].ToArray());
				ptr_vari++;
				remainingData = remainingData[ptr_vari..];
			}

			// produced cargo
			ProducedCargo.Clear();
			ProducedCargo = SawyerStreamReader.LoadVariableHeaders(remainingData, 2);
			remainingData = remainingData[(S5Header.StructLength * 2)..];

			// required cargo
			RequiredCargo.Clear();
			RequiredCargo = SawyerStreamReader.LoadVariableHeaders(remainingData, 2);
			remainingData = remainingData[(S5Header.StructLength * 2)..];

			// load ??
			var_AE.Clear();
			for (var i = 0; i < var_AD; ++i)
			{
				var size = BitConverter.ToUInt16(remainingData[..2]);
				remainingData = remainingData[2..];

				var_AE.Add(remainingData[..size].ToArray());
				remainingData = remainingData[size..];
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
		{
			var ms = new MemoryStream();

			foreach (var x in PartHeights)
			{
				ms.WriteByte(x);
			}

			foreach (var x in PartAnimations)
			{
				ms.Write(BitConverter.GetBytes(x));
			}

			foreach (var x in Parts)
			{
				ms.Write(x);
				ms.WriteByte(0xFF);
			}

			foreach (var obj in ProducedCargo.Concat(RequiredCargo))
			{
				ms.Write(obj.Write());
			}

			foreach (var unk in var_AE)
			{
				ms.Write(BitConverter.GetBytes((uint16_t)unk.Length));
				foreach (var x in unk)
				{
					ms.Write(BitConverter.GetBytes((uint16_t)x));
				}
			}

			return ms.ToArray();
		}
	}
}
