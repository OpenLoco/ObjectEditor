using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Building;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.BuildingObjectLoader;

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

	public static class ImageGroups
	{
		public const int Base = 4;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new BuildingObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipImageId(); // Image offset, not part of object definition
			var numBuildingParts = br.ReadByte();
			var numBuildingVariations = br.ReadByte();
			br.SkipPointer(); // BuildingHeights, not part of object definition
			br.SkipPointer(); // BuildingAnimations, not part of object definition
			br.SkipPointer(Constants.BuildingVariationCount); // BuildingVariations, not part of object definition
			model.Colours = br.ReadUInt32();
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			model.Flags = ((DatBuildingObjectFlags)br.ReadByte()).Convert();
			model.CostIndex = br.ReadByte();
			model.SellCostFactor = br.ReadUInt16();
			model.ScaffoldingSegmentType = br.ReadByte();
			model.ScaffoldingColour = (Colour)br.ReadByte();
			model.GeneratorFunction = br.ReadByte();
			model.AverageNumberOnMap = br.ReadByte();
			model.ProducedQuantity.Add(br.ReadByte());
			model.ProducedQuantity.Add(br.ReadByte());
			br.SkipObjectId(Constants.MaxProducedCargoType);
			br.SkipObjectId(Constants.MaxRequiredCargoType);
			model.var_A6 = br.ReadByte();
			model.var_A7 = br.ReadByte();
			model.var_A8 = br.ReadByte();
			model.var_A9 = br.ReadByte();
			model.DemolishRatingReduction = br.ReadInt16();
			model.var_AC = br.ReadByte();
			var numElevatorSequences = br.ReadByte();
			br.SkipPointer(Constants.MaxElevatorHeightSequences); // ElevatorHeightSequences, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Building), null);

			// variable
			LoadVariable(br, model, numBuildingParts, numBuildingVariations, numElevatorSequences);

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = CreateImageTable(imageList);

			return new LocoObject(ObjectType.Building, model, stringTable, imageTable);
		}
	}

	private static ImageTable CreateImageTable(List<GraphicsElement> imageList)
	{
		var imageTable = new ImageTable();

		var chunks = imageList.Chunk(4);
		imageTable.Groups.Add(("Base", chunks.First().ToList()));
		var floorCount = 0;
		foreach (var chunk in chunks.Skip(1))
		{
			imageTable.Groups.Add(($"Floor {floorCount++}", chunk.ToList()));
		}

		return imageTable;
	}

	private static void LoadVariable(LocoBinaryReader br, BuildingObject model, byte numBuildingParts, byte numBuildingVariations, byte numElevatorSequences)
	{
		model.BuildingHeights = br.ReadBuildingHeights(numBuildingParts);
		model.BuildingAnimations = br.ReadBuildingAnimations(numBuildingParts);
		model.BuildingVariations = br.ReadBuildingVariations(numBuildingVariations);
		model.ProducedCargo = br.ReadS5HeaderList(Constants.MaxProducedCargoType);
		model.RequiredCargo = br.ReadS5HeaderList(Constants.MaxRequiredCargoType);

		// elevator sequences
		for (var i = 0; i < numElevatorSequences; ++i)
		{
			var size = br.ReadUInt16();
			var sequence = br.ReadBytes(size);
			model.ElevatorHeightSequences.Add(sequence);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (BuildingObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.Write((uint8_t)model.BuildingAnimations.Count); // NumBuildingParts
			bw.Write((uint8_t)model.BuildingVariations.Count);
			bw.WriteEmptyPointer();
			bw.WriteEmptyPointer();
			bw.WriteEmptyPointer(Constants.BuildingVariationCount);
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
			bw.WriteEmptyObjectId(Constants.MaxProducedCargoType);
			bw.WriteEmptyObjectId(Constants.MaxRequiredCargoType);
			bw.Write(model.var_A6);
			bw.Write(model.var_A7);
			bw.Write(model.var_A8);
			bw.Write(model.var_A9);
			bw.Write(model.DemolishRatingReduction);
			bw.Write(model.var_AC);
			bw.Write((uint8_t)model.ElevatorHeightSequences.Count);
			bw.WriteEmptyPointer(Constants.MaxElevatorHeightSequences); // ElevatorHeightSequences, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	private static void SaveVariable(BuildingObject model, LocoBinaryWriter bw)
	{
		bw.Write(model.BuildingHeights);
		bw.Write(model.BuildingAnimations);
		bw.Write(model.BuildingVariations);
		bw.WriteS5HeaderList(model.ProducedCargo, Constants.MaxProducedCargoType);
		bw.WriteS5HeaderList(model.RequiredCargo, Constants.MaxRequiredCargoType);

		// elevator sequences
		foreach (var unk in model.ElevatorHeightSequences)
		{
			bw.Write((uint16_t)unk.Length);
			bw.Write(unk);
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
}

internal static class BuildingObjectFlagsConverter
{
	public static BuildingObjectFlags Convert(this DatBuildingObjectFlags datBuildingObjectFlags)
		=> (BuildingObjectFlags)datBuildingObjectFlags;

	public static DatBuildingObjectFlags Convert(this BuildingObjectFlags buildingObjectFlags)
		=> (DatBuildingObjectFlags)buildingObjectFlags;
}
