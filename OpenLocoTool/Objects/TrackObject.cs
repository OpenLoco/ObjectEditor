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
	public record TrackObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] TrackObjectPieceFlags TrackPieces,
		[property: LocoStructOffset(0x04)] uint16_t StationTrackPieces,
		[property: LocoStructOffset(0x06)] uint8_t var_06,
		[property: LocoStructOffset(0x07)] uint8_t NumCompatible,
		[property: LocoStructOffset(0x08)] uint8_t NumMods,
		[property: LocoStructOffset(0x09)] uint8_t NumSignals,
		[property: LocoStructOffset(0x0A), LocoArrayLength(4)] uint8_t[] Mods,
		[property: LocoStructOffset(0x0E)] uint16_t Signals, // bitset
		[property: LocoStructOffset(0x10)] uint16_t CompatibleTracks, // bitset
		[property: LocoStructOffset(0x12)] uint16_t CompatibleRoads, // bitset
		[property: LocoStructOffset(0x14)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x16)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x18)] int16_t TunnelCostFactor,
		[property: LocoStructOffset(0x1A)] uint8_t CostIndex,
		[property: LocoStructOffset(0x1B)] uint8_t Tunnel,
		[property: LocoStructOffset(0x1C)] uint16_t CurveSpeed,
		[property: LocoStructOffset(0x1E)] uint32_t Image,
		[property: LocoStructOffset(0x22)] TrackObjectFlags Flags,
		[property: LocoStructOffset(0x24)] uint8_t NumBridges,
		[property: LocoStructOffset(0x25), LocoArrayLength(7)] uint8_t[] Bridges,        // 0x25
		[property: LocoStructOffset(0x2C)] uint8_t NumStations,
		[property: LocoStructOffset(0x2D), LocoArrayLength(7)] uint8_t[] Stations,       // 0x2D
		[property: LocoStructOffset(0x34)] uint8_t DisplayOffset,
		[property: LocoStructOffset(0x35)] uint8_t pad_35
	) : ILocoStruct, ILocoStructVariableData
	{
		public static ObjectType ObjectType => ObjectType.Track;
		public static int StructSize => 0x36;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// compatible roads/tracks
			remainingData = remainingData[(S5Header.StructLength * NumCompatible)..];

			// mods
			remainingData = remainingData[(S5Header.StructLength * NumMods)..];
			// tunnel
			remainingData = remainingData[(S5Header.StructLength * NumSignals)..];

			// tunnel
			remainingData = remainingData[(S5Header.StructLength * 1)..];

			// bridges
			remainingData = remainingData[(S5Header.StructLength * NumBridges)..];

			// stations
			remainingData = remainingData[(S5Header.StructLength * NumStations)..];

			return remainingData;
		}

		public ReadOnlySpan<byte> Save() => throw new NotImplementedException();
	}
}