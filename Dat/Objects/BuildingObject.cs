using Common;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using System.ComponentModel;

namespace Dat.Objects;

[Flags]
public enum BuildingObjectFlags : uint8_t
{
	None = 0,
	LargeTile = 1 << 0, // 2x2 tile
	MiscBuilding = 1 << 1,
	Indestructible = 1 << 2,
	IsHeadquarters = 1 << 3,
}

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0xBE)]
[LocoStructType(ObjectType.Building)]
[LocoStringTable("Name")]
public record BuildingObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x06)] uint8_t NumBuildingParts,
		[property: LocoStructOffset(0x07)] uint8_t NumBuildingVariations,
		[property: LocoStructOffset(0x08), LocoStructVariableLoad, LocoArrayLength(BuildingObject.BuildingHeightCount)] List<uint8_t> BuildingHeights,
		[property: LocoStructOffset(0x0C), LocoStructVariableLoad, LocoArrayLength(BuildingObject.BuildingAnimationCount)] List<BuildingPartAnimation> BuildingAnimations,
		[property: LocoStructOffset(0x10), LocoStructVariableLoad, LocoArrayLength(BuildingObject.BuildingVariationCount)] List<List<uint8_t>> BuildingVariations,
		[property: LocoStructOffset(0x90)] uint32_t Colours,
		[property: LocoStructOffset(0x94)] uint16_t DesignedYear,
		[property: LocoStructOffset(0x96)] uint16_t ObsoleteYear,
		[property: LocoStructOffset(0x98)] BuildingObjectFlags Flags,
		[property: LocoStructOffset(0x99)] uint8_t CostIndex,
		[property: LocoStructOffset(0x9A)] uint16_t SellCostFactor,
		[property: LocoStructOffset(0x9C)] uint8_t ScaffoldingSegmentType,
		[property: LocoStructOffset(0x9D)] Colour ScaffoldingColour,
		[property: LocoStructOffset(0x9E)] uint8_t GeneratorFunction,
		[property: LocoStructOffset(0x9F)] uint8_t AverageNumberOnMap,
		[property: LocoStructOffset(0xA0), LocoArrayLength(BuildingObject.MaxProducedCargoType)] uint8_t[] ProducedQuantity,
		[property: LocoStructOffset(0xA2), LocoStructVariableLoad, LocoArrayLength(BuildingObject.MaxProducedCargoType)] List<S5Header> ProducedCargo,
		[property: LocoStructOffset(0xA4), LocoStructVariableLoad, LocoArrayLength(BuildingObject.MaxRequiredCargoType)] List<S5Header> RequiredCargo,
		[property: LocoStructOffset(0xA6), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> var_A6,
		[property: LocoStructOffset(0xA8), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> var_A8,
		[property: LocoStructOffset(0xAA)] int16_t DemolishRatingReduction,
		[property: LocoStructOffset(0xAC)] uint8_t var_AC,
		[property: LocoStructOffset(0xAD)] uint8_t NumElevatorSequences,
		[property: LocoStructOffset(0xAE), LocoStructVariableLoad, LocoArrayLength(BuildingObject.MaxElevatorHeightSequences)] List<uint8_t[]> _ElevatorHeightSequences // 0xAE ->0xB2->0xB6->0xBA->0xBE (4 byte pointers)
	) : ILocoStruct, ILocoStructVariableData
{
	public const int BuildingVariationCount = 32;
	public const int BuildingHeightCount = 4;
	public const int BuildingAnimationCount = 2;
	public const int MaxProducedCargoType = 2;
	public const int MaxRequiredCargoType = 2;
	public const int MaxElevatorHeightSequences = 4;

	public List<uint8_t[]> ElevatorHeightSequences { get; set; } = [];

	// known issues:
	// HOSPITL1.dat - loads without error but graphics are bugged
	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// variation heights
		BuildingHeights.Clear();
		BuildingHeights.AddRange(ByteReaderT.Read_Array<uint8_t>(remainingData[..(NumBuildingParts * 1)], NumBuildingParts));
		remainingData = remainingData[(NumBuildingParts * 1)..]; // uint8_t*

		// variation animations
		BuildingAnimations.Clear();
		var buildingAnimationSize = ObjectAttributes.StructSize<BuildingPartAnimation>();
		BuildingAnimations.AddRange(ByteReader.ReadLocoStructArray(remainingData[..(NumBuildingParts * buildingAnimationSize)], typeof(BuildingPartAnimation), NumBuildingParts, buildingAnimationSize)
			.Cast<BuildingPartAnimation>());
		remainingData = remainingData[(NumBuildingParts * 2)..]; // uint16_t*

		// variation parts
		BuildingVariations.Clear();
		for (var i = 0; i < NumBuildingVariations; ++i)
		{
			var ptr_10 = 0;
			while (remainingData[++ptr_10] != 0xFF)
			{ }

			BuildingVariations.Add(remainingData[..ptr_10].ToArray().ToList());
			ptr_10++;
			remainingData = remainingData[ptr_10..];
		}

		// produced cargo
		ProducedCargo.Clear();
		ProducedCargo.AddRange(SawyerStreamReader.LoadVariableCountS5Headers(remainingData, MaxProducedCargoType));
		remainingData = remainingData[(S5Header.StructLength * MaxProducedCargoType)..];

		// required cargo
		RequiredCargo.Clear();
		RequiredCargo.AddRange(SawyerStreamReader.LoadVariableCountS5Headers(remainingData, MaxRequiredCargoType));
		remainingData = remainingData[(S5Header.StructLength * MaxRequiredCargoType)..];

		// animation sequences
		ElevatorHeightSequences.Clear();
		for (var i = 0; i < NumElevatorSequences; ++i)
		{
			var size = BitConverter.ToUInt16(remainingData[..2]);
			remainingData = remainingData[2..];

			ElevatorHeightSequences.Add(remainingData[..size].ToArray());
			remainingData = remainingData[size..];
		}

		return remainingData;
	}

	public ReadOnlySpan<byte> SaveVariable()
	{
		var ms = new MemoryStream();

		// variation heights
		foreach (var x in BuildingHeights)
		{
			ms.WriteByte(x);
		}

		// variation animations
		foreach (var x in BuildingAnimations)
		{
			ms.WriteByte(x.NumFrames);
			ms.WriteByte(x.AnimationSpeed);
		}

		// variation parts
		foreach (var x in BuildingVariations)
		{
			ms.Write(x.ToArray());
			ms.WriteByte(0xFF);
		}

		// produced cargo
		foreach (var obj in ProducedCargo.Fill(MaxProducedCargoType, S5Header.NullHeader))
		{
			ms.Write(obj!.Write());
		}

		// required cargo
		foreach (var obj in RequiredCargo.Fill(MaxRequiredCargoType, S5Header.NullHeader))
		{
			ms.Write(obj!.Write());
		}

		foreach (var unk in ElevatorHeightSequences)
		{
			ms.Write(BitConverter.GetBytes((uint16_t)unk.Length));
			foreach (var x in unk)
			{
				ms.Write(BitConverter.GetBytes((uint16_t)x));
			}
		}

		return ms.ToArray();
	}

	public bool Validate()
		=> NumBuildingParts is not 0 and not > 63
		&& NumBuildingVariations is not 0 and <= 31;
}
