using Common;
using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class AirportObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int BuildingVariationCount = 32;
		public const int BuildingHeightCount = 4;
		public const int BuildingAnimationCount = 2;
	}

	internal static class StructSizes
	{
		public const int Dat = 0xBA;
		public const int BuildingPartAnimation = 0x02;
		public const int AirportBuilding = 0x04;
		public const int MovementNode = 0x08;
		public const int MovementEdge = 0x0C;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

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

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

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
			br.ReadBytes(StructSizes.BuildingPartAnimation * numBuildingParts),
			numBuildingParts,
			StructSizes.BuildingPartAnimation);
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
			var building = ByteReader.ReadLocoStruct<AirportBuilding>(br.ReadBytes(StructSizes.AirportBuilding));
			model.BuildingPositions.Add(building);
		}
		_ = br.ReadByte(); // Consume the 0xFF terminator

		// movement nodes
		var movementNodes = ByteReader.ReadLocoStructArray<MovementNode>(
			br.ReadBytes(StructSizes.MovementNode * numMovementNodes),
			numMovementNodes,
			StructSizes.MovementNode);
		model.MovementNodes = [.. movementNodes.Cast<MovementNode>()];

		// movement edges
		var movementEdges = ByteReader.ReadLocoStructArray<MovementEdge>(
			br.ReadBytes(StructSizes.MovementEdge * numMovementEdges),
			numMovementEdges,
			StructSizes.MovementEdge);
		model.MovementEdges = [.. movementEdges.Cast<MovementEdge>()];
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
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
			bw.Write((uint8_t)model.BuildingHeights.Count);
			bw.Write((uint8_t)model.BuildingVariations.Count);
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

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			SaveVariable(stream, model);

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}

	private static void SaveVariable(MemoryStream stream, AirportObject model)
	{
		// heights
		foreach (var x in model.BuildingHeights)
		{
			stream.WriteByte(x);
		}

		// animations
		foreach (var x in model.BuildingAnimations)
		{
			stream.WriteByte(x.NumFrames);
			stream.WriteByte(x.AnimationSpeed);
		}

		// variations
		foreach (var x in model.BuildingVariations)
		{
			stream.Write(x.ToArray());
			stream.WriteByte(0xFF);
		}

		// positions
		foreach (var x in model.BuildingPositions)
		{
			stream.WriteByte(x.Index);
			stream.WriteByte(x.Rotation);
			stream.WriteByte((byte)x.X);
			stream.WriteByte((byte)x.Y);
		}

		stream.WriteByte(0xFF);

		// movement nodes
		foreach (var x in model.MovementNodes)
		{
			stream.Write(BitConverter.GetBytes(x.X));
			stream.Write(BitConverter.GetBytes(x.Y));
			stream.Write(BitConverter.GetBytes(x.Z));
			stream.Write(BitConverter.GetBytes((uint16_t)x.Flags));
		}

		// movement edges
		foreach (var x in model.MovementEdges)
		{
			stream.WriteByte(x.var_00);
			stream.WriteByte(x.CurrNode);
			stream.WriteByte(x.NextNode);
			stream.WriteByte(x.var_03);
			stream.Write(BitConverter.GetBytes(x.MustBeClearEdges));
			stream.Write(BitConverter.GetBytes(x.AtLeastOneClearEdges));
		}
	}
}
