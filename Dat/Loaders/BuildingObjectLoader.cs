using Common;
using Dat.Converters;
using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class BuildingObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int BuildingVariationCount = 32;
		public const int BuildingHeightCount = 4;
		public const int BuildingAnimationCount = 2;
		public const int MaxProducedCargoType = 2;
		public const int MaxRequiredCargoType = 2;
		public const int MaxElevatorHeightSequences = 4;
	}

	public static class StructSizes
	{
		public const int Dat = 0xBE;
		public const int BuildingPartAnimation = 0x02;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new BuildingObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId(); // Name offset, not part of object definition
			_ = br.SkipImageId(); // Image offset, not part of object definition
			var numBuildingParts = br.ReadByte();
			var numBuildingVariations = br.ReadByte();
			_ = br.SkipPointer(); // BuildingHeights, not part of object definition
			_ = br.SkipPointer(); // BuildingAnimations, not part of object definition
			_ = br.SkipPointer(Constants.BuildingVariationCount); // BuildingVariations, not part of object definition
			model.Colours = br.ReadUInt32();
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			model.Flags = (BuildingObjectFlags)br.ReadByte();
			model.CostIndex = br.ReadByte();
			model.SellCostFactor = br.ReadUInt16();
			model.ScaffoldingSegmentType = br.ReadByte();
			model.ScaffoldingColour = (Colour)br.ReadByte();
			model.GeneratorFunction = br.ReadByte();
			model.AverageNumberOnMap = br.ReadByte();
			model.ProducedQuantity.Add(br.ReadByte());
			model.ProducedQuantity.Add(br.ReadByte());
			_ = br.SkipObjectId(Constants.MaxProducedCargoType);
			_ = br.SkipObjectId(Constants.MaxRequiredCargoType);
			model.var_A6 = br.ReadByte();
			model.var_A7 = br.ReadByte();
			model.var_A8 = br.ReadByte();
			model.var_A9 = br.ReadByte();
			model.DemolishRatingReduction = br.ReadInt16();
			model.var_AC = br.ReadByte();
			var numElevatorSequences = br.ReadByte();
			_ = br.SkipPointer(Constants.MaxElevatorHeightSequences); // ElevatorHeightSequences, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Building), null);

			// variable
			LoadVariable(br, model, numBuildingParts, numBuildingVariations, numElevatorSequences);

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Building, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, BuildingObject model, byte numBuildingParts, byte numBuildingVariations, byte numElevatorSequences)
	{
		// building heights
		model.BuildingHeights = [.. br.ReadBytes(numBuildingParts)];

		// building animations
		var buildingAnimationStructs = ByteReader.ReadLocoStructArray<BuildingPartAnimation>(
			br.ReadBytes(StructSizes.BuildingPartAnimation * numBuildingParts),
			numBuildingParts,
			StructSizes.BuildingPartAnimation);
		model.BuildingAnimations = [.. buildingAnimationStructs.Cast<BuildingPartAnimation>()];

		// building variations
		for (var i = 0; i < numBuildingVariations; ++i)
		{
			List<byte> tmp = [];
			byte b;
			while ((b = br.ReadByte()) != 0xFF)
			{
				tmp.Add(b);
			}

			model.BuildingVariations.Add(tmp);
		}

		// produced cargo
		model.ProducedCargo = br.ReadS5HeaderList(Constants.MaxProducedCargoType);

		// required cargo
		model.RequiredCargo = br.ReadS5HeaderList(Constants.MaxRequiredCargoType);

		// animation sequences
		for (var i = 0; i < numElevatorSequences; ++i)
		{
			var size = br.ReadUInt16();
			var sequence = br.ReadBytes(size);
			model.ElevatorHeightSequences.Add(sequence);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (BuildingObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition
			bw.WriteImageId(); // Image offset, not part of object definition
			bw.Write((uint8_t)model.BuildingAnimations.Count); // NumBuildingParts
			bw.Write((uint8_t)model.BuildingVariations.Count);
			bw.WritePointer();
			bw.WritePointer();
			bw.WritePointer(Constants.BuildingVariationCount);
			bw.Write(model.Colours);
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);
			bw.Write((uint8_t)model.Flags);
			bw.Write(model.CostIndex);
			bw.Write(model.SellCostFactor);
			bw.Write(model.ScaffoldingSegmentType);
			bw.Write((uint8_t)model.ScaffoldingColour);
			bw.Write(model.GeneratorFunction);
			bw.Write(model.AverageNumberOnMap);
			bw.Write(model.ProducedQuantity[0]);
			bw.Write(model.ProducedQuantity[1]);
			bw.WriteObjectId(Constants.MaxProducedCargoType);
			bw.WriteObjectId(Constants.MaxRequiredCargoType);
			bw.Write(model.var_A6);
			bw.Write(model.var_A7);
			bw.Write(model.var_A8);
			bw.Write(model.var_A9);
			bw.Write(model.DemolishRatingReduction);
			bw.Write(model.var_AC);
			bw.Write((uint8_t)model.ElevatorHeightSequences.Count);
			bw.WritePointer(Constants.MaxElevatorHeightSequences); // ElevatorHeightSequences, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}

	private static void SaveVariable(BuildingObject model, LocoBinaryWriter bw)
	{
		bw.Write(model.BuildingHeights.ToArray());

		foreach (var x in model.BuildingAnimations)
		{
			bw.Write(x.NumFrames);
			bw.Write(x.AnimationSpeed);
		}

		foreach (var x in model.BuildingVariations)
		{
			bw.Write(x.ToArray());
			bw.Write((byte)0xFF);
		}

		foreach (var x in model.ProducedCargo
			.Select(y => y.Convert())
			.Fill(Constants.MaxProducedCargoType, S5Header.NullHeader))
		{
			bw.Write(x!.Write());
		}

		foreach (var x in model.RequiredCargo
			.Select(y => y.Convert())
			.Fill(Constants.MaxRequiredCargoType, S5Header.NullHeader))
		{
			bw.Write(x!.Write());
		}

		foreach (var unk in model.ElevatorHeightSequences)
		{
			bw.Write((uint16_t)unk.Length);
			foreach (var x in unk)
			{
				bw.Write((uint16_t)x);
			}
		}
	}
}

[Flags]
internal enum DatBuildingObjectFlags : uint8_t
{
	None = 0,
	LargeTile = 1 << 0, // 2x2 tile
	MiscBuilding = 1 << 1,
	Indestructible = 1 << 2,
	IsHeadquarters = 1 << 3,
}

[LocoStructSize(0xBE)]
[LocoStructType(DatObjectType.Building)]
internal record DatBuildingObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), Browsable(false)] image_id Image,
		[property: LocoStructOffset(0x06)] uint8_t NumBuildingParts,
		[property: LocoStructOffset(0x07)] uint8_t NumBuildingVariations,
		[property: LocoStructOffset(0x08), LocoStructVariableLoad, LocoArrayLength(BuildingObjectLoader.Constants.BuildingHeightCount)] List<uint8_t> BuildingHeights,
		[property: LocoStructOffset(0x0C), LocoStructVariableLoad, LocoArrayLength(BuildingObjectLoader.Constants.BuildingAnimationCount)] List<BuildingPartAnimation> BuildingAnimations,
		[property: LocoStructOffset(0x10), LocoStructVariableLoad, LocoArrayLength(BuildingObjectLoader.Constants.BuildingVariationCount)] List<List<uint8_t>> BuildingVariations,
		[property: LocoStructOffset(0x90)] uint32_t Colours,
		[property: LocoStructOffset(0x94)] uint16_t DesignedYear,
		[property: LocoStructOffset(0x96)] uint16_t ObsoleteYear,
		[property: LocoStructOffset(0x98)] DatBuildingObjectFlags Flags,
		[property: LocoStructOffset(0x99)] uint8_t CostIndex,
		[property: LocoStructOffset(0x9A)] uint16_t SellCostFactor,
		[property: LocoStructOffset(0x9C)] uint8_t ScaffoldingSegmentType,
		[property: LocoStructOffset(0x9D)] DatColour ScaffoldingColour,
		[property: LocoStructOffset(0x9E)] uint8_t GeneratorFunction,
		[property: LocoStructOffset(0x9F)] uint8_t AverageNumberOnMap,
		[property: LocoStructOffset(0xA0), LocoArrayLength(BuildingObjectLoader.Constants.MaxProducedCargoType)] uint8_t[] ProducedQuantity,
		[property: LocoStructOffset(0xA2), LocoStructVariableLoad, LocoArrayLength(BuildingObjectLoader.Constants.MaxProducedCargoType)] List<S5Header> ProducedCargo,
		[property: LocoStructOffset(0xA4), LocoStructVariableLoad, LocoArrayLength(BuildingObjectLoader.Constants.MaxRequiredCargoType)] List<S5Header> RequiredCargo,
		[property: LocoStructOffset(0xA6), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> var_A6,
		[property: LocoStructOffset(0xA8), LocoStructVariableLoad, LocoArrayLength(2)] List<uint8_t> var_A8,
		[property: LocoStructOffset(0xAA)] int16_t DemolishRatingReduction,
		[property: LocoStructOffset(0xAC)] uint8_t var_AC,
		[property: LocoStructOffset(0xAD)] uint8_t NumElevatorSequences,
		[property: LocoStructOffset(0xAE), LocoStructVariableLoad, LocoArrayLength(BuildingObjectLoader.Constants.MaxElevatorHeightSequences)] List<uint8_t[]> _ElevatorHeightSequences // 0xAE ->0xB2->0xB6->0xBA->0xBE (4 byte pointers)
	) : ILocoStructVariableData
{
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
		ProducedCargo.AddRange(SawyerStreamReader.ReadS5HeaderList(remainingData, BuildingObjectLoader.Constants.MaxProducedCargoType));
		remainingData = remainingData[(S5Header.StructLength * BuildingObjectLoader.Constants.MaxProducedCargoType)..];

		// required cargo
		RequiredCargo.Clear();
		RequiredCargo.AddRange(SawyerStreamReader.ReadS5HeaderList(remainingData, BuildingObjectLoader.Constants.MaxRequiredCargoType));
		remainingData = remainingData[(S5Header.StructLength * BuildingObjectLoader.Constants.MaxRequiredCargoType)..];

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
		foreach (var obj in ProducedCargo.Fill(BuildingObjectLoader.Constants.MaxProducedCargoType, S5Header.NullHeader))
		{
			ms.Write(obj!.Write());
		}

		// required cargo
		foreach (var obj in RequiredCargo.Fill(BuildingObjectLoader.Constants.MaxRequiredCargoType, S5Header.NullHeader))
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
}
