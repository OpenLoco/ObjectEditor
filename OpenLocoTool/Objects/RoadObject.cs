
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
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
		Intersection = 1 << 2,
		OneSided = 1 << 5,
		Overtake = 1 << 6,
		StreetLights = 1 << 8,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record RoadObject(
		[property: LocoStructProperty] string_id Name,
		[property: LocoStructProperty] RoadObjectPieceFlags RoadPieces, // 0x02
		[property: LocoStructProperty] int16_t BuildCostFactor,         // 0x04
		[property: LocoStructProperty] int16_t SellCostFactor,          // 0x06
		[property: LocoStructProperty] int16_t TunnelCostFactor,        // 0x08
		[property: LocoStructProperty] uint8_t CostIndex,               // 0x0A
		[property: LocoStructProperty] uint8_t Tunnel,                  // 0x0B
		[property: LocoStructProperty] Speed16 MaxSpeed,                // 0x0C
		[property: LocoStructProperty] uint32_t Image,                  // 0x0E
		[property: LocoStructProperty] RoadObjectFlags Flags,           // 0x12
		[property: LocoStructProperty] uint8_t NumBridges,              // 0x14
		[property: LocoStructProperty, LocoArrayLength(7)] uint8_t[] Bridges,              // 0x15
		[property: LocoStructProperty] uint8_t NumStations,             // 0x1C
		[property: LocoStructProperty, LocoArrayLength(7)] uint8_t[] Stations,             // 0x1D
		[property: LocoStructProperty] uint8_t PaintStyle,              // 0x24
		[property: LocoStructProperty] uint8_t NumMods,                 // 0x25
		[property: LocoStructProperty, LocoArrayLength(2)] uint8_t[] Mods,                 // 0x26
		[property: LocoStructProperty] uint8_t NumCompatible,           // 0x28
		[property: LocoStructProperty] uint8_t pad_29,
		[property: LocoStructProperty] uint16_t CompatibleRoads,  // 0x2A
		[property: LocoStructProperty] uint16_t CompatibleTracks, // 0x2C
		[property: LocoStructProperty] uint8_t TargetTownSize,   // 0x2E
		[property: LocoStructProperty] uint8_t pad_2F
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.road;
		public int ObjectStructSize => 0x30;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
