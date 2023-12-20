
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
		Intersection = 1 << 4,
		OneSided = 1 << 5,
		Overtake = 1 << 6,
		StreetLights = 1 << 8,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x30)]
	public record RoadObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] RoadObjectPieceFlags RoadPieces,
		[property: LocoStructOffset(0x04)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x06)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x08)] int16_t TunnelCostFactor,
		[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
		[property: LocoStructOffset(0x0B)] uint8_t Tunnel,
		[property: LocoStructOffset(0x0C)] Speed16 MaxSpeed,
		[property: LocoStructOffset(0x0E)] uint32_t Image,
		[property: LocoStructOffset(0x12)] RoadObjectFlags Flags,
		[property: LocoStructOffset(0x14)] uint8_t NumBridges,
		[property: LocoStructOffset(0x15), LocoArrayLength(7)] uint8_t[] Bridges,
		[property: LocoStructOffset(0x1C)] uint8_t NumStations,
		[property: LocoStructOffset(0x1D), LocoArrayLength(7)] uint8_t[] Stations,
		[property: LocoStructOffset(0x24)] uint8_t PaintStyle,
		[property: LocoStructOffset(0x25)] uint8_t NumMods,
		[property: LocoStructOffset(0x26), LocoArrayLength(2)] uint8_t[] Mods,
		[property: LocoStructOffset(0x28)] uint8_t NumCompatible,
		[property: LocoStructOffset(0x29)] uint8_t pad_29,
		[property: LocoStructOffset(0x2A)] uint16_t CompatibleRoads,
		[property: LocoStructOffset(0x2C)] uint16_t CompatibleTracks,
		[property: LocoStructOffset(0x2E)] uint8_t TargetTownSize,
		[property: LocoStructOffset(0x2F)] uint8_t pad_2F
		) : ILocoStruct, ILocoStructVariableData
	{
		public static ObjectType ObjectType => ObjectType.Road;
		public static int StructSize => 0x30;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// compatible roads/tracks
			remainingData = remainingData[(S5Header.StructLength * NumCompatible)..];

			// mods
			remainingData = remainingData[(S5Header.StructLength * NumMods)..];

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
