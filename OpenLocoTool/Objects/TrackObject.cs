using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	public enum TrackObjectPieceFlags : uint16_t
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
	};

	public enum TrackObjectFlags : uint16_t
	{
		None = 0,
		unk_00 = 1 << 0,
		unk_01 = 1 << 1,
		unk_02 = 1 << 2,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x36)]
	[LocoStructType(ObjectType.Track)]
	[LocoStringTable("Name")]
	public class TrackObject(
		TrackObjectPieceFlags trackPieces,
		uint16_t stationTrackPieces,
		uint8_t var_06,
		uint8_t numCompatible,
		uint8_t numMods,
		uint8_t numSignals,
		int16_t buildCostFactor,
		int16_t sellCostFactor,
		int16_t tunnelCostFactor,
		uint8_t costIndex,
		uint16_t curveSpeed,
		TrackObjectFlags flags,
		uint8_t numBridges,
		uint8_t numStations,
		uint8_t displayOffset)
		: ILocoStruct, ILocoStructVariableData
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] public string_id Name,
		[LocoStructOffset(0x02)] public TrackObjectPieceFlags TrackPieces { get; set; } = trackPieces;
		[LocoStructOffset(0x04)] public uint16_t StationTrackPieces { get; set; } = stationTrackPieces;
		[LocoStructOffset(0x06)] public uint8_t var_06 { get; set; } = var_06;
		[LocoStructOffset(0x07)] public uint8_t NumCompatible { get; set; } = numCompatible;
		[LocoStructOffset(0x08)] public uint8_t NumMods { get; set; } = numMods;
		[LocoStructOffset(0x09)] public uint8_t NumSignals { get; set; } = numSignals;
		//[LocoStructOffset(0x0A), LocoArrayLength(4)] public object_index[] Mods { get; set; }
		//[LocoStructOffset(0x0E)] public uint16_t Signals { get; set; } = signals; // bitset

		//[LocoStructOffset(0x10)] public uint16_t CompatibleTracks { get; set; } // bitset
		//[LocoStructOffset(0x12)] public uint16_t CompatibleRoads { get; set; } // bitset
		[LocoStructOffset(0x14)] public int16_t BuildCostFactor { get; set; } = buildCostFactor;
		[LocoStructOffset(0x16)] public int16_t SellCostFactor { get; set; } = sellCostFactor;
		[LocoStructOffset(0x18)] public int16_t TunnelCostFactor { get; set; } = tunnelCostFactor;
		[LocoStructOffset(0x1A)] public uint8_t CostIndex { get; set; } = costIndex;
		//[LocoStructOffset(0x1B)] public object_index Tunnel { get; set; }
		[LocoStructOffset(0x1C)] public uint16_t CurveSpeed { get; set; } = curveSpeed;
		//[LocoStructOffset(0x1E)] image_id Image,
		[LocoStructOffset(0x22)] public TrackObjectFlags Flags { get; set; } = flags;
		[LocoStructOffset(0x24)] public uint8_t NumBridges { get; set; } = numBridges;
		//[LocoStructOffset(0x25), LocoArrayLength(7)] object_index[] Bridges { get; set; }        // 0x25
		[LocoStructOffset(0x2C)] public uint8_t NumStations { get; set; } = numStations;
		//[LocoStructOffset(0x2D), LocoArrayLength(7)] object_index[] Stations { get; set; }       // 0x2D
		[LocoStructOffset(0x34)] public uint8_t DisplayOffset { get; set; } = displayOffset;
		//[LocoStructOffset(0x35)] public uint8_t pad_35 { get; set; } = pad_35;

		public List<S5Header> Compatible { get; set; } = [];
		public List<S5Header> Mods { get; set; } = [];
		public List<S5Header> Signals { get; set; } = [];
		public S5Header Tunnel { get; set; }
		public List<S5Header> Bridges { get; set; } = [];
		public List<S5Header> Stations { get; set; } = [];

		public const int NumTunnels = 1;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			Compatible.Clear();
			Mods.Clear();
			Signals.Clear();
			Bridges.Clear();
			Stations.Clear();

			// compatible roads/tracks
			Compatible = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumCompatible);
			remainingData = remainingData[(S5Header.StructLength * NumCompatible)..];

			// mods
			Mods = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumMods);
			remainingData = remainingData[(S5Header.StructLength * NumMods)..];

			// signals
			Signals = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumSignals);
			remainingData = remainingData[(S5Header.StructLength * NumSignals)..];

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