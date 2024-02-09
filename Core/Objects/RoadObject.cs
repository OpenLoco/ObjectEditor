using System.ComponentModel;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;

namespace OpenLocoObjectEditor.Objects
{
	[Flags]
	public enum RoadObjectFlags : uint16_t
	{
		none = 0,
		unk_00 = 1 << 0,
		unk_01 = 1 << 1,
		unk_02 = 1 << 2,
		unk_03 = 1 << 3, // Likely isTram
		unk_04 = 1 << 4,
		unk_05 = 1 << 5,
		IsRoad = 1 << 6, // If not set this is tram track
	};

	[Flags]
	public enum RoadObjectPieceFlags : uint16_t
	{
		None = 0,
		OneWay = 1 << 0,
		Track = 1 << 1,
		Slope = 1 << 2,
		SteepSlope = 1 << 3,
		Intersection = 1 << 4,
		OneSided = 1 << 5,
		Overtake = 1 << 6,
		StreetLights = 1 << 8,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x30)]
	[LocoStructType(ObjectType.Road)]
	[LocoStringTable("Name")]
	public class RoadObject(
		RoadObjectPieceFlags roadPieces,
		int16_t buildCostFactor,
		int16_t sellCostFactor,
		int16_t tunnelCostFactor,
		uint8_t costIndex,
		int16_t maxSpeed,
		RoadObjectFlags flags,
		uint8_t numBridges,
		uint8_t numStations,
		uint8_t paintStyle,
		uint8_t numMods,
		uint8_t numCompatible,
		uint8_t targetTownSize)
		: ILocoStruct, ILocoStructVariableData
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] public string_id Name,
		[LocoStructOffset(0x02)] public RoadObjectPieceFlags RoadPieces { get; set; } = roadPieces;
		[LocoStructOffset(0x04)] public int16_t BuildCostFactor { get; set; } = buildCostFactor;
		[LocoStructOffset(0x06)] public int16_t SellCostFactor { get; set; } = sellCostFactor;
		[LocoStructOffset(0x08)] public int16_t TunnelCostFactor { get; set; } = tunnelCostFactor;
		[LocoStructOffset(0x0A)] public uint8_t CostIndex { get; set; } = costIndex;
		//[LocoStructOffset(0x0B)] public object_index Tunnel { get; set; }
		[LocoStructOffset(0x0C)] public Speed16 MaxSpeed { get; set; } = maxSpeed;
		//[LocoStructOffset(0x0E)] public image_id Image,
		[LocoStructOffset(0x12)] public RoadObjectFlags Flags { get; set; } = flags;
		[LocoStructOffset(0x14)] public uint8_t NumBridges { get; set; } = numBridges;
		//[LocoStructOffset(0x15), LocoArrayLength(7)] object_index[] Bridges { get; set; }
		[LocoStructOffset(0x1C)] public uint8_t NumStations { get; set; } = numStations;
		//[LocoStructOffset(0x1D), LocoArrayLength(7)] object_index[] Stations { get; set; }
		[LocoStructOffset(0x24)] public uint8_t PaintStyle { get; set; } = paintStyle;
		[LocoStructOffset(0x25)] public uint8_t NumMods { get; set; } = numMods;
		//[LocoStructOffset(0x26), LocoArrayLength(2)] object_index[] Mods { get; set; }
		[LocoStructOffset(0x28)] public uint8_t NumCompatible { get; set; } = numCompatible;
		//[LocoStructOffset(0x29)] public uint8_t pad_29 { get; set; } = pad_29;
		//[LocoStructOffset(0x2A)] public uint16_t CompatibleRoads { get; set; } // bitset
		//[LocoStructOffset(0x2C)] public uint16_t CompatibleTracks { get; set; } // bitset
		[LocoStructOffset(0x2E)] public uint8_t TargetTownSize { get; set; } = targetTownSize;
		//[LocoStructOffset(0x2F)] public uint8_t pad_2F { get; set; } = pad_2F;

		public List<S5Header> Compatible { get; set; } = [];
		public List<S5Header> Mods { get; set; } = [];
		public S5Header Tunnel { get; set; }
		public List<S5Header> Bridges { get; set; } = [];
		public List<S5Header> Stations { get; set; } = [];

		public const int NumTunnels = 1;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// compatible roads/tracks
			Compatible = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumCompatible);
			remainingData = remainingData[(S5Header.StructLength * NumCompatible)..];

			// mods
			Mods = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumMods);
			remainingData = remainingData[(S5Header.StructLength * NumMods)..];

			// tunnel
			Tunnel = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumTunnels)[0];
			remainingData = remainingData[(S5Header.StructLength * NumTunnels)..];

			// bridges
			Bridges = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumBridges);
			remainingData = remainingData[(S5Header.StructLength * NumBridges)..];

			// stations
			Stations = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumStations);
			remainingData = remainingData[(S5Header.StructLength * NumStations)..];

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
		{
			//var data = new byte[S5Header.StructLength * (NumCompatible + NumMods + 1 + NumBridges + NumStations)];

			var headers = Compatible
				.Concat(Mods)
				.Concat(Enumerable.Repeat(Tunnel, 1))
				.Concat(Bridges)
				.Concat(Stations);

			return headers.SelectMany(h => h.Write().ToArray()).ToArray();
		}
	}
}
