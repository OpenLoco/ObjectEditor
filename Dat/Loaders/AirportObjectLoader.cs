using Common;
using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class AirportObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int BuildingVariationCount = 32;
		public const int BuildingHeightCount = 4;
		public const int BuildingAnimationCount = 2;

	}

	public static class Sizes
	{
		public const int BuildingPartAnimation = 2;
		public const int AirportBuilding = 4;
		public const int MovementNode = 8;
		public const int MovementEdge = 12;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		using (var br = new LocoBinaryReader(stream))
		{
			var model = new AirportObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId(); // Name offset, not part of object definition
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.var_07 = br.ReadByte();
			_ = br.SkipImageId(); // Image, not part of object definition
			_ = br.SkipImageId(); // Image offset, not part of object definition
			model.AllowedPlaneTypes = br.ReadUInt16();
			var numBuildingParts = br.ReadByte();
			var numBuildingVariations = br.ReadByte();
			_ = br.SkipPointer(); // BuildingHeights
			_ = br.SkipPointer(); // BuildingAnimations
			_ = br.SkipPointer(Constants.BuildingVariationCount); // BuildingVariations
			_ = br.SkipPointer(); // BuildingPositions
			model.LargeTiles = br.ReadUInt32();
			model.MinX = br.ReadSByte();
			model.MinY = br.ReadSByte();
			model.MaxX = br.ReadSByte();
			model.MaxY = br.ReadSByte();
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			var numMovementNodes = br.ReadByte();
			var numMovementEdges = br.ReadByte();
			_ = br.SkipPointer(); // MovementNodes
			_ = br.SkipPointer(); // MovementEdges
			model.var_B6 = br.ReadBytes(0xBA - 0xB6);

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Airport), null);

			// variable
			LoadVariable(br, model, numBuildingParts, numBuildingVariations, numMovementNodes, numMovementEdges);

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Airport, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, AirportObject model, int numBuildingParts, int numBuildingVariations, byte numMovementNodes, byte numMovementEdges)
	{
		// variation heights
		model.BuildingHeights = [.. br.ReadBytes(numBuildingParts)];

		// variation animations
		var buildingAnimationStructs = ByteReader.ReadLocoStructArray<BuildingPartAnimation>(
			br.ReadBytes(Sizes.BuildingPartAnimation * numBuildingParts),
			numBuildingParts,
			Sizes.BuildingPartAnimation);
		model.BuildingAnimations = [.. buildingAnimationStructs.Cast<BuildingPartAnimation>()];

		// variation variations
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

		// building positions
		while (br.PeekByte() != 0xFF)
		{
			var building = ByteReader.ReadLocoStruct<AirportBuilding>(br.ReadBytes(Sizes.AirportBuilding));
			model.BuildingPositions.Add(building);
		}
		_ = br.ReadByte(); // Consume the 0xFF terminator

		// movement nodes
		var movementNodes = ByteReader.ReadLocoStructArray<MovementNode>(
			br.ReadBytes(Sizes.MovementNode * numMovementNodes),
			numMovementNodes,
			Sizes.MovementNode);
		model.MovementNodes = [.. movementNodes.Cast<MovementNode>()];

		// movement edges
		var movementEdges = ByteReader.ReadLocoStructArray<MovementEdge>(
			br.ReadBytes(Sizes.MovementEdge * numMovementEdges),
			numMovementEdges,
			Sizes.MovementEdge);
		model.MovementEdges = [.. movementEdges.Cast<MovementEdge>()];
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var model = obj.Object as AirportObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId();// Name offset, not part of object definition
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write(model.var_07);
			bw.WriteImageId(); // Image, not part of object definition
			bw.WriteImageId(); // Image offset, not part of object definition
			bw.Write(model.AllowedPlaneTypes);
			bw.Write(model.BuildingHeights.Count);
			bw.Write(model.BuildingVariations.Count);
			bw.WritePointer(); // BuildingHeights
			bw.WritePointer(); // BuildingAnimations
			bw.WritePointer(Constants.BuildingVariationCount); // BuildingVariations
			bw.WritePointer(); // BuildingPositions
			bw.Write(model.LargeTiles);
			bw.Write(model.MinX);
			bw.Write(model.MinY);
			bw.Write(model.MaxX);
			bw.Write(model.MaxY);
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);
			bw.Write((uint8_t)model.MovementNodes.Count);
			bw.Write((uint8_t)model.MovementEdges.Count);
			bw.WritePointer(); // MovementNodes
			bw.WritePointer(); // MovementEdges
			bw.Write(model.var_B6);

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			SaveVariable(stream, model);

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}

	private static void SaveVariable(MemoryStream ms, AirportObject model)
	{
		// heights
		foreach (var x in model.BuildingHeights)
		{
			ms.WriteByte(x);
		}

		// animations
		foreach (var x in model.BuildingAnimations)
		{
			ms.WriteByte(x.NumFrames);
			ms.WriteByte(x.AnimationSpeed);
		}

		// variations
		foreach (var x in model.BuildingVariations)
		{
			ms.Write(x.ToArray());
			ms.WriteByte(0xFF);
		}

		// positions
		foreach (var x in model.BuildingPositions)
		{
			ms.WriteByte(x.Index);
			ms.WriteByte(x.Rotation);
			ms.WriteByte((byte)x.X);
			ms.WriteByte((byte)x.Y);
		}

		ms.WriteByte(0xFF);

		// movement nodes
		foreach (var x in model.MovementNodes)
		{
			ms.Write(BitConverter.GetBytes(x.X));
			ms.Write(BitConverter.GetBytes(x.Y));
			ms.Write(BitConverter.GetBytes(x.Z));
			ms.Write(BitConverter.GetBytes((uint16_t)x.Flags));
		}

		// movement edges
		foreach (var x in model.MovementEdges)
		{
			ms.WriteByte(x.var_00);
			ms.WriteByte(x.CurrNode);
			ms.WriteByte(x.NextNode);
			ms.WriteByte(x.var_03);
			ms.Write(BitConverter.GetBytes(x.MustBeClearEdges));
			ms.Write(BitConverter.GetBytes(x.AtLeastOneClearEdges));
		}
	}
}

[LocoStructSize(0xBA)]
[LocoStructType(DatObjectType.Airport)]
public record DatAirportObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] int16_t BuildCostFactor,
	[property: LocoStructOffset(0x04)] int16_t SellCostFactor,
	[property: LocoStructOffset(0x06)] uint8_t CostIndex,
	[property: LocoStructOffset(0x07)] uint8_t var_07,
	[property: LocoStructOffset(0x08), Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x0C), Browsable(false)] image_id ImageOffset,
	[property: LocoStructOffset(0x10)] uint16_t AllowedPlaneTypes,
	[property: LocoStructOffset(0x12)] uint8_t NumBuildingParts,
	[property: LocoStructOffset(0x13)] uint8_t NumBuildingVariations,
	[property: LocoStructOffset(0x14), LocoStructVariableLoad, LocoArrayLength(AirportObjectLoader.Constants.BuildingHeightCount)] List<uint8_t> BuildingHeights,
	[property: LocoStructOffset(0x18), LocoStructVariableLoad, LocoArrayLength(AirportObjectLoader.Constants.BuildingAnimationCount)] List<BuildingPartAnimation> BuildingAnimations,
	[property: LocoStructOffset(0x1C), LocoStructVariableLoad, LocoArrayLength(AirportObjectLoader.Constants.BuildingVariationCount)] List<List<uint8_t>> BuildingVariations,
	[property: LocoStructOffset(0x9C), LocoStructVariableLoad] List<AirportBuilding> BuildingPositions,
	[property: LocoStructOffset(0xA0)] uint32_t LargeTiles,
	[property: LocoStructOffset(0xA4)] int8_t MinX,
	[property: LocoStructOffset(0xA5)] int8_t MinY,
	[property: LocoStructOffset(0xA6)] int8_t MaxX,
	[property: LocoStructOffset(0xA7)] int8_t MaxY,
	[property: LocoStructOffset(0xA8)] uint16_t DesignedYear,
	[property: LocoStructOffset(0xAA)] uint16_t ObsoleteYear,
	[property: LocoStructOffset(0xAC)] uint8_t NumMovementNodes,
	[property: LocoStructOffset(0xAD)] uint8_t NumMovementEdges,
	[property: LocoStructOffset(0xAE), LocoStructVariableLoad] List<MovementNode> MovementNodes,
	[property: LocoStructOffset(0xB2), LocoStructVariableLoad] List<MovementEdge> MovementEdges,
	[property: LocoStructOffset(0xB6), LocoArrayLength(0xBA - 0xB6)] uint8_t[] var_B6
)
{
}
