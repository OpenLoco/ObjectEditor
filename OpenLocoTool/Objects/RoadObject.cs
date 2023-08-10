
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
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] RoadObjectPieceFlags RoadPieces,
		[property: LocoStructProperty(0x04)] int16_t BuildCostFactor,
		[property: LocoStructProperty(0x06)] int16_t SellCostFactor,
		[property: LocoStructProperty(0x08)] int16_t TunnelCostFactor,
		[property: LocoStructProperty(0x0A)] uint8_t CostIndex,
		[property: LocoStructProperty(0x0B)] uint8_t Tunnel,
		[property: LocoStructProperty(0x0C)] Speed16 MaxSpeed,
		[property: LocoStructProperty(0x0E)] uint32_t Image,
		[property: LocoStructProperty(0x12)] RoadObjectFlags Flags,
		[property: LocoStructProperty(0x14)] uint8_t NumBridges,
		[property: LocoStructProperty(0x15), LocoArrayLength(7)] uint8_t[] Bridges,
		[property: LocoStructProperty(0x1C)] uint8_t NumStations,
		[property: LocoStructProperty(0x1D), LocoArrayLength(7)] uint8_t[] Stations,
		[property: LocoStructProperty(0x24)] uint8_t PaintStyle,
		[property: LocoStructProperty(0x25)] uint8_t NumMods,
		[property: LocoStructProperty(0x26), LocoArrayLength(2)] uint8_t[] Mods,
		[property: LocoStructProperty(0x28)] uint8_t NumCompatible,
		[property: LocoStructProperty(0x29)] uint8_t pad_29,
		[property: LocoStructProperty(0x2A)] uint16_t CompatibleRoads,
		[property: LocoStructProperty(0x2C)] uint16_t CompatibleTracks,
		[property: LocoStructProperty(0x2E)] uint8_t TargetTownSize,
		[property: LocoStructProperty(0x2F)] uint8_t pad_2F
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.road;
		public static int ObjectStructSize => 0x30;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}
