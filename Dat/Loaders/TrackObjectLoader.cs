using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.TrackObjectLoader;

namespace Dat.Loaders;

public abstract class TrackObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MaxTunnels = 1;
		public const int MaxBridges = 7;
		public const int MaxStations = 7;
		public const int MaxMods = 4;
	}

	public static ObjectType ObjectType => ObjectType.Track;
	public static DatObjectType DatObjectType => DatObjectType.Track;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new TrackObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.TrackPieces = ((DatTrackTraitFlags)br.ReadUInt16()).Convert();
			model.StationTrackPieces = ((DatTrackTraitFlags)br.ReadUInt16()).Convert();
			model.var_06 = br.ReadByte();
			var numCompatibleTracksAndRoads = br.ReadByte();
			var numTrackMods = br.ReadByte();
			var numSignals = br.ReadByte();
			br.SkipObjectId(Constants.MaxMods);
			br.SkipUInt16(); // _Signals, not part of object definition
			br.SkipUInt16(); // _CompatibleTracks, not part of object definition
			br.SkipUInt16(); // _CompatibleRoads, not part of object definition
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.TunnelCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			br.SkipObjectId(); // Tunnel, not part of object definition
			model.MaxCurveSpeed = br.ReadInt16();
			br.SkipImageId(); // Image offset, not part of object definition
			model.Flags = ((DatTrackObjectFlags)br.ReadUInt16()).Convert();
			var numBridges = br.ReadByte();
			br.SkipObjectId(Constants.MaxBridges);
			var numStations = br.ReadByte();
			br.SkipObjectId(Constants.MaxStations);
			model.DisplayOffset = br.ReadByte();
			model.pad_35 = br.ReadByte(); // pad_35, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			model.TracksAndRoads = [.. br.ReadS5HeaderList(numCompatibleTracksAndRoads)];
			model.TrackMods = [.. br.ReadS5HeaderList(numTrackMods)];
			model.Signals = [.. br.ReadS5HeaderList(numSignals)];
			model.Tunnel = br.ReadS5Header();
			model.Bridges = [.. br.ReadS5HeaderList(numBridges)];
			model.Stations = [.. br.ReadS5HeaderList(numStations)];

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableGrouper.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (TrackObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write((uint16_t)model.TrackPieces.Convert());
			bw.Write((uint16_t)model.StationTrackPieces.Convert());
			bw.Write(model.var_06);
			bw.Write((uint8_t)model.TracksAndRoads.Count);
			bw.Write((uint8_t)model.TrackMods.Count);
			bw.Write((uint8_t)model.Signals.Count);
			bw.WriteEmptyObjectId(Constants.MaxMods); // Mods, not part of object definition
			bw.Write((uint16_t)0); // _Signals, not part of object definition
			bw.Write((uint16_t)0); // _CompatibleTracks, not part of object definition
			bw.Write((uint16_t)0); // _CompatibleRoads, not part of object definition
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.TunnelCostFactor);
			bw.Write(model.CostIndex);
			bw.WriteEmptyObjectId(); // Tunnel, not part of object definition
			bw.Write(model.MaxCurveSpeed);
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.Write((uint16_t)model.Flags.Convert());
			bw.Write((uint8_t)model.Bridges.Count);
			bw.WriteEmptyObjectId(Constants.MaxBridges); // Bridges, not part of object definition
			bw.Write((uint8_t)model.Stations.Count);
			bw.WriteEmptyObjectId(Constants.MaxStations); // Stations, not part of object definition
			bw.Write(model.DisplayOffset);
			bw.Write(model.pad_35); // pad_35, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			bw.WriteS5HeaderList(model.TracksAndRoads);
			bw.WriteS5HeaderList(model.TrackMods);
			bw.WriteS5HeaderList(model.Signals);
			bw.WriteS5Header(model.Tunnel);
			bw.WriteS5HeaderList(model.Bridges);
			bw.WriteS5HeaderList(model.Stations);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	[Flags]
	internal enum DatTrackTraitFlags : uint16_t
	{
		None = 0,
		Diagonal = 1 << 0,
		LargeCurve = 1 << 1,
		NormalCurve = 1 << 2,
		SmallCurve = 1 << 3,
		VerySmallCurve = 1 << 4,
		Slope = 1 << 5,
		SteepSlope = 1 << 6,
		OneSided = 1 << 7,
		SlopedCurve = 1 << 8,
		SBend = 1 << 9,
		Junction = 1 << 10,
	}

	[Flags]
	internal enum DatTrackObjectFlags : uint16_t
	{
		None = 0,
		unk_00 = 1 << 0,
		unk_01 = 1 << 1,
		unk_02 = 1 << 2,
	}
}

internal static class TrackTraitFlagsConverter
{
	public static TrackTraitFlags Convert(this DatTrackTraitFlags datTrackTraitFlags)
		=> (TrackTraitFlags)datTrackTraitFlags;

	public static DatTrackTraitFlags Convert(this TrackTraitFlags trackTraitFlags)
		=> (DatTrackTraitFlags)trackTraitFlags;
}

internal static class TrackObjectFlagsConverter
{
	public static TrackObjectFlags Convert(this DatTrackObjectFlags datTrackObjectFlags)
		=> (TrackObjectFlags)datTrackObjectFlags;

	public static DatTrackObjectFlags Convert(this TrackObjectFlags trackObjectFlags)
		=> (DatTrackObjectFlags)trackObjectFlags;
}
