using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

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

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new RoadObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId(); // Name offset, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Road), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Road, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
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

[LocoStructSize(0x30)]
[LocoStructType(DatObjectType.Road)]
internal record DatRoadObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] DatRoadTraitFlags RoadPieces,
	[property: LocoStructOffset(0x04)] int16_t BuildCostFactor,
	[property: LocoStructOffset(0x06)] int16_t SellCostFactor,
	[property: LocoStructOffset(0x08)] int16_t TunnelCostFactor,
	[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
	[property: LocoStructOffset(0x0B), Browsable(false)] object_id _Tunnel,
	[property: LocoStructOffset(0x0C)] Speed16 MaxSpeed,
	[property: LocoStructOffset(0x0E), Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x12)] DatRoadObjectFlags Flags,
	[property: LocoStructOffset(0x14)] uint8_t NumBridges,
	[property: LocoStructOffset(0x15), LocoArrayLength(RoadObjectLoader.Constants.MaxBridges), Browsable(false)] object_id[] _Bridges,
	[property: LocoStructOffset(0x1C)] uint8_t NumStations,
	[property: LocoStructOffset(0x1D), LocoArrayLength(RoadObjectLoader.Constants.MaxStations), Browsable(false)] object_id[] _Stations,
	[property: LocoStructOffset(0x24)] uint8_t PaintStyle,
	[property: LocoStructOffset(0x25)] uint8_t NumMods,
	[property: LocoStructOffset(0x26), LocoArrayLength(RoadObjectLoader.Constants.MaxMods), Browsable(false)] object_id[] _Mods,
	[property: LocoStructOffset(0x28)] uint8_t NumCompatible,
	[property: LocoStructOffset(0x29)] uint8_t DisplayOffset,
	[property: LocoStructOffset(0x2A), Browsable(false)] uint16_t _CompatibleRoads, // bitset
	[property: LocoStructOffset(0x2C), Browsable(false)] uint16_t _CompatibleTracks, // bitset
	[property: LocoStructOffset(0x2E)] DatTownSize TargetTownSize,
	[property: LocoStructOffset(0x2F), Browsable(false)] uint8_t pad_2F
	) : ILocoStruct, ILocoStructVariableData
{
	public List<S5Header> Compatible { get; set; } = [];
	public List<S5Header> Mods { get; set; } = [];
	public S5Header Tunnel { get; set; }
	public List<S5Header> Bridges { get; set; } = [];
	public List<S5Header> Stations { get; set; } = [];

	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// compatible roads/tracks
		Compatible = SawyerStreamReader.ReadS5HeaderList(remainingData, NumCompatible);
		remainingData = remainingData[(S5Header.StructLength * NumCompatible)..];

		// mods
		Mods = SawyerStreamReader.ReadS5HeaderList(remainingData, NumMods);
		remainingData = remainingData[(S5Header.StructLength * NumMods)..];

		// tunnel
		Tunnel = SawyerStreamReader.ReadS5HeaderList(remainingData, RoadObjectLoader.Constants.MaxTunnels)[0];
		remainingData = remainingData[(S5Header.StructLength * RoadObjectLoader.Constants.MaxTunnels)..];

		// bridges
		Bridges = SawyerStreamReader.ReadS5HeaderList(remainingData, NumBridges);
		remainingData = remainingData[(S5Header.StructLength * NumBridges)..];

		// stations
		Stations = SawyerStreamReader.ReadS5HeaderList(remainingData, NumStations);
		remainingData = remainingData[(S5Header.StructLength * NumStations)..];

		// set _CompatibleRoads?
		// set _CompatibleTracks?

		return remainingData;
	}

	public ReadOnlySpan<byte> SaveVariable()
	{
		//var data = new byte[S5Header.StructLength * (NumCompatible + NumMods + 1 + NumBridges + NumStations)];

		var headers = Compatible
			.Concat(Mods)
			.Concat(Enumerable.Repeat(Tunnel, 1))
			.Concat(Bridges)
			.Concat(Stations);

		return headers.SelectMany(h => h.Write().ToArray()).ToArray();
	}

	public bool Validate()
	{
		// check missing in vanilla
		if (CostIndex >= 32)
		{
			return false;
		}

		if (-SellCostFactor > BuildCostFactor)
		{
			return false;
		}

		if (BuildCostFactor <= 0)
		{
			return false;
		}

		if (TunnelCostFactor <= 0)
		{
			return false;
		}

		if (NumBridges > 7)
		{
			return false;
		}

		if (NumMods > 2)
		{
			return false;
		}

		if (Flags.HasFlag(DatRoadObjectFlags.unk_03))
		{
			return NumMods == 0;
		}

		return true;
	}
}
