using Common;
using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Airport;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.AirportObjectLoader;
using static Dat.Loaders.TrackObjectLoader;

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

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new AirportObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.var_07 = br.ReadByte();
			br.SkipImageId(); // Image, not part of object definition
			br.SkipImageId(); // Image offset, not part of object definition
			model.AllowedPlaneTypes = br.ReadUInt16();
			var numBuildingParts = br.ReadByte();
			var numBuildingVariations = br.ReadByte();
			br.SkipPointer(); // BuildingHeights
			br.SkipPointer(); // BuildingAnimations
			br.SkipPointer(Constants.BuildingVariationCount); // BuildingVariations
			br.SkipPointer(); // BuildingPositions
			model.LargeTiles = br.ReadUInt32();
			model.MinX = br.ReadSByte();
			model.MinY = br.ReadSByte();
			model.MaxX = br.ReadSByte();
			model.MaxY = br.ReadSByte();
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			var numMovementNodes = br.ReadByte();
			var numMovementEdges = br.ReadByte();
			br.SkipPointer(); // MovementNodes
			br.SkipPointer(); // MovementEdges
			model.var_B6 = br.ReadBytes(0xBA - 0xB6);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Airport), null);

			// variable
			LoadVariable(br, model, numBuildingParts, numBuildingVariations, numMovementNodes, numMovementEdges);

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.Airport, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, AirportObject model, int numBuildingParts, int numBuildingVariations, byte numMovementNodes, byte numMovementEdges)
	{
		model.BuildingHeights = br.ReadBuildingHeights(numBuildingParts);
		model.BuildingAnimations = br.ReadBuildingAnimations(numBuildingParts);
		model.BuildingVariations = br.ReadBuildingVariations(numBuildingVariations);

		// building positions
		while (br.PeekByte() != LocoConstants.Terminator)
		{
			var building = ByteReader.ReadLocoStruct<AirportBuilding>(br.ReadBytes(StructSizes.AirportBuilding));
			model.BuildingPositions.Add(building);
		}
		br.SkipTerminator();

		// movement nodes
		for (var i = 0; i < numMovementNodes; ++i)
		{
			var mn = new MovementNode()
			{
				Flags = ((DatAirportMovementNodeFlags)br.ReadUInt16()).Convert(),
				Position = new Pos3(br.ReadInt16(), br.ReadInt16(), br.ReadInt16()),
			};
			model.MovementNodes.Add(mn);
		}

		// movement edges
		for (var i = 0; i < numMovementEdges; ++i)
		{
			var me = new MovementEdge()
			{
				var_00 = br.ReadByte(),
				CurrNode = br.ReadByte(),
				NextNode = br.ReadByte(),
				var_03 = br.ReadByte(),
				MustBeClearEdges = br.ReadUInt32(),
				AtLeastOneClearEdges = br.ReadUInt32(),
			};
			model.MovementEdges.Add(me);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = obj.Object as AirportObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId();// Name offset, not part of object definition
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write(model.var_07);
			bw.WriteEmptyImageId(); // Image, not part of object definition
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.Write(model.AllowedPlaneTypes);
			bw.Write((uint8_t)model.BuildingHeights.Count);
			bw.Write((uint8_t)model.BuildingVariations.Count);
			bw.WriteEmptyPointer(); // BuildingHeights
			bw.WriteEmptyPointer(); // BuildingAnimations
			bw.WriteEmptyPointer(Constants.BuildingVariationCount); // BuildingVariations
			bw.WriteEmptyPointer(); // BuildingPositions
			bw.Write(model.LargeTiles);
			bw.Write(model.MinX);
			bw.Write(model.MinY);
			bw.Write(model.MaxX);
			bw.Write(model.MaxY);
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);
			bw.Write((uint8_t)model.MovementNodes.Count);
			bw.Write((uint8_t)model.MovementEdges.Count);
			bw.WriteEmptyPointer(); // MovementNodes
			bw.WriteEmptyPointer(); // MovementEdges
			bw.Write(model.var_B6);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			SaveVariable(bw, model);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.GraphicsElements);
		}
	}

	private static void SaveVariable(LocoBinaryWriter bw, AirportObject model)
	{
		bw.Write(model.BuildingHeights);
		bw.Write(model.BuildingAnimations);
		bw.Write(model.BuildingVariations);

		// positions
		foreach (var x in model.BuildingPositions)
		{
			bw.Write(x.Index);
			bw.Write(x.Rotation);
			bw.Write(x.X);
			bw.Write(x.Y);
		}
		bw.WriteTerminator();

		// movement nodes
		foreach (var x in model.MovementNodes)
		{
			bw.Write(x.Position.X);
			bw.Write(x.Position.Y);
			bw.Write(x.Position.Z);
			bw.Write((uint16_t)x.Flags);
		}

		// movement edges
		foreach (var x in model.MovementEdges)
		{
			bw.Write(x.var_00);
			bw.Write(x.CurrNode);
			bw.Write(x.NextNode);
			bw.Write(x.var_03);
			bw.Write(x.MustBeClearEdges);
			bw.Write(x.AtLeastOneClearEdges);
		}
	}

	[Flags]
	public enum DatAirportMovementNodeFlags : uint16_t
	{
		None = 0,
		Terminal = 1 << 0,
		TakeoffEnd = 1 << 1,
		Flag2 = 1 << 2,
		Taxiing = 1 << 3,
		InFlight = 1 << 4,
		HeliTakeoffBegin = 1 << 5,
		TakeoffBegin = 1 << 6,
		HeliTakeoffEnd = 1 << 7,
		Touchdown = 1 << 8,
	}
}

internal static class AirportMovementNodeFlagsConverter
{
	public static AirportMovementNodeFlags Convert(this DatAirportMovementNodeFlags datAirportMovementNodeFlags)
		=> (AirportMovementNodeFlags)datAirportMovementNodeFlags;

	public static DatAirportMovementNodeFlags Convert(this AirportMovementNodeFlags airportMovementNodeFlags)
		=> (DatAirportMovementNodeFlags)airportMovementNodeFlags;
}
