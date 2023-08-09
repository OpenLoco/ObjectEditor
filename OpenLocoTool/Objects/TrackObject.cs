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
	public record TrackObject(
		[property: LocoStructProperty] string_id Name,
		[property: LocoStructProperty] TrackObjectPieceFlags TrackPieces, // 0x02
		[property: LocoStructProperty] uint16_t StationTrackPieces,       // 0x04
		[property: LocoStructProperty] uint8_t var_06,
		[property: LocoStructProperty] uint8_t NumCompatible,     // 0x07
		[property: LocoStructProperty] uint8_t NumMods,           // 0x08
		[property: LocoStructProperty] uint8_t NumSignals,        // 0x09
		[property: LocoStructProperty, LocoArrayLength(4)] uint8_t[] Mods,           // 0x0A
		[property: LocoStructProperty] uint16_t Signals,          // 0x0E bitset
		[property: LocoStructProperty] uint16_t CompatibleTracks, // 0x10 bitset
		[property: LocoStructProperty] uint16_t CompatibleRoads,  // 0x12 bitset
		[property: LocoStructProperty] int16_t BuildCostFactor,   // 0x14
		[property: LocoStructProperty] int16_t SellCostFactor,    // 0x16
		[property: LocoStructProperty] int16_t TunnelCostFactor,  // 0x18
		[property: LocoStructProperty] uint8_t CostIndex,         // 0x1A
		[property: LocoStructProperty] uint8_t Tunnel,            // 0x1B
		[property: LocoStructProperty] uint16_t CurveSpeed,       // 0x1C
		[property: LocoStructProperty] uint32_t Image,            // 0x1E
		[property: LocoStructProperty] TrackObjectFlags Flags,    // 0x22
		[property: LocoStructProperty] uint8_t NumBridges,        // 0x24
		[property: LocoStructProperty, LocoArrayLength(7)] uint8_t[] Bridges,        // 0x25
		[property: LocoStructProperty] uint8_t NumStations,       // 0x2C
		[property: LocoStructProperty, LocoArrayLength(7)] uint8_t[] Stations,       // 0x2D
		[property: LocoStructProperty] uint8_t DisplayOffset,     // 0x34
		[property: LocoStructProperty] uint8_t pad_35
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.track;
		public int ObjectStructSize => 0x36;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}