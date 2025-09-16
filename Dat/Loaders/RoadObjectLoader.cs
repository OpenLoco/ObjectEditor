using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.RoadObjectLoader;

namespace Dat.Loaders;

public abstract class RoadObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MaxTunnels = 1;
		public const int MaxBridges = 7;
		public const int MaxStations = 7;
		public const int MaxMods = 2;
	}

	public static class StructSizes
	{
		public const int Dat = 0x30;
	}

	public static ObjectType ObjectType => ObjectType.Road;
	public static DatObjectType DatObjectType => DatObjectType.Road;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new RoadObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.RoadPieces = ((DatRoadTraitFlags)br.ReadUInt16()).Convert();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.TunnelCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			br.SkipObjectId(); // Tunnel offset, not part of object definition
			model.MaxCurveSpeed = br.ReadInt16();
			br.SkipImageId(); // Image offset, not part of object definition
			model.Flags = ((DatRoadObjectFlags)br.ReadUInt16()).Convert();
			var numBridges = br.ReadByte();
			br.SkipObjectId(Constants.MaxBridges);
			var numStations = br.ReadByte();
			br.SkipObjectId(Constants.MaxStations);
			model.PaintStyle = br.ReadByte();
			var numMods = br.ReadByte();
			br.SkipObjectId(Constants.MaxMods);
			var numCompatible = br.ReadByte();
			model.DisplayOffset = br.ReadByte();
			br.SkipUInt16(); // _CompatibleRoads, not part of object definition
			br.SkipUInt16(); // _CompatibleTracks, not part of object definition
			model.TargetTownSize = ((DatTownSize)br.ReadByte()).Convert();
			br.SkipByte(); // pad_2F, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			LoadVariable(br, model, numBridges, numStations, numMods, numCompatible);

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableLoader.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, RoadObject model, byte numBridges, byte numStations, byte numRoadMods, byte numCompatibleTracksAndRoads)
	{
		model.CompatibleTracksAndRoads = br.ReadS5HeaderList(numCompatibleTracksAndRoads);
		model.RoadMods = br.ReadS5HeaderList(numRoadMods);
		model.Tunnel = br.ReadS5Header();
		model.Bridges = br.ReadS5HeaderList(numBridges);
		model.Stations = br.ReadS5HeaderList(numStations);
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (RoadObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write((uint16_t)model.RoadPieces.Convert());
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.TunnelCostFactor);
			bw.Write(model.CostIndex);
			bw.WriteEmptyObjectId();
			bw.Write(model.MaxCurveSpeed);
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.Write((uint16_t)model.Flags.Convert());
			bw.Write((uint8_t)model.Bridges.Count);
			bw.WriteEmptyObjectId(Constants.MaxBridges);
			bw.Write((uint8_t)model.Stations.Count);
			bw.WriteEmptyObjectId(Constants.MaxStations);
			bw.Write(model.PaintStyle);
			bw.Write((uint8_t)model.RoadMods.Count);
			bw.WriteEmptyObjectId(Constants.MaxMods);
			bw.Write((uint8_t)model.CompatibleTracksAndRoads.Count);
			bw.Write(model.DisplayOffset);
			bw.Write((uint16_t)0); // _CompatibleRoads, not part of object definition
			bw.Write((uint16_t)0); // _CompatibleTracks, not part of object definition
			bw.Write((uint8_t)model.TargetTownSize.Convert());
			bw.Write((uint8_t)0); // pad_2F, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			bw.WriteS5HeaderList(model.CompatibleTracksAndRoads);
			bw.WriteS5HeaderList(model.RoadMods);
			bw.WriteS5Header(model.Tunnel);
			bw.WriteS5HeaderList(model.Bridges);
			bw.WriteS5HeaderList(model.Stations);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	[Flags]
	internal enum DatRoadObjectFlags : uint16_t
	{
		None = 0,
		IsOneWay = 1 << 0,
		unk_01 = 1 << 1,
		unk_02 = 1 << 2,
		unk_03 = 1 << 3, // Likely isTram
		unk_04 = 1 << 4,
		unk_05 = 1 << 5,
		IsRoad = 1 << 6, // If not set this is tram track
		unk_07 = 1 << 7,
		unk_08 = 1 << 8,
	}

	internal enum DatTownSize : uint8_t
	{
		Hamlet,
		Village,
		Town,
		City,
		Metropolis,
	}

	[Flags]
	internal enum DatRoadTraitFlags : uint16_t
	{
		None = 0,
		SmallCurve = 1 << 0,
		VerySmallCurve = 1 << 1,
		Slope = 1 << 2,
		SteepSlope = 1 << 3,
		unk_04 = 1 << 4, // intersection?
		Turnaround = 1 << 5,
		unk_06 = 1 << 6, // overtake?
		unk_07 = 1 << 7,
		unk_08 = 1 << 8, // streetlight?
	}
}

static class RoadObjectFlagsConverter
{
	public static DatRoadObjectFlags Convert(this RoadObjectFlags roadObjectFlags)
		=> (DatRoadObjectFlags)roadObjectFlags;

	public static RoadObjectFlags Convert(this DatRoadObjectFlags datRoadObjectFlags)
		=> (RoadObjectFlags)datRoadObjectFlags;
}

static class RoadTraitFlagsConverter
{
	public static DatRoadTraitFlags Convert(this RoadTraitFlags roadTraitFlags)
		=> (DatRoadTraitFlags)roadTraitFlags;

	public static RoadTraitFlags Convert(this DatRoadTraitFlags datRoadTraitFlags)
		=> (RoadTraitFlags)datRoadTraitFlags;
}

static class TownSizeConverter
{
	public static DatTownSize Convert(this TownSize townSize)
		=> (DatTownSize)townSize;

	public static TownSize Convert(this DatTownSize datTownSize)
		=> (TownSize)datTownSize;
}
