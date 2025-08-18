using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Bridge;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class BridgeObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int _03PadSize = 3;
		public const int MaxNumTrackMods = 7;
		public const int MaxNumRoadMods = 7;

	}

	public static LocoObject Load(MemoryStream stream)
	{
		using (var br = new LocoBinaryReader(stream))
		{
			var model = new BridgeObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Bridge), null);

			// variable
			//LoadVariable(br, model, numBuildingParts, numBuildingVariations, numMovementNodes, numMovementEdges);

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Bridge, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{

	}
}

[Flags]
internal enum DatBridgeDisabledTrackFlags : uint16_t
{
	None = 0,
	Slope = 1 << 0,
	SteepSlope = 1 << 1,
	CurveSlope = 1 << 2,
	Diagonal = 1 << 3,
	VerySmallCurve = 1 << 4,
	SmallCurve = 1 << 5,
	Curve = 1 << 6,
	LargeCurve = 1 << 7,
	SBendCurve = 1 << 8,
	OneSided = 1 << 9,
	StartsAtHalfHeight = 1 << 10, // Not used. From RCT2
	Junction = 1 << 11,
}

[Flags]
internal enum DatBridgeObjectFlags : uint8_t
{
	None = 0,
	HasRoof = 1 << 0,
}

[LocoStructSize(0x2C)]
[LocoStructType(DatObjectType.Bridge)]
internal record DatBridgeObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] DatBridgeObjectFlags Flags,
	[property: LocoStructOffset(0x03), LocoStructVariableLoad] uint8_t var_03,
	[property: LocoStructOffset(0x04)] uint16_t ClearHeight,
	[property: LocoStructOffset(0x06)] int16_t DeckDepth,
	[property: LocoStructOffset(0x08)] uint8_t SpanLength,
	[property: LocoStructOffset(0x09)] uint8_t PillarSpacing, // this is a bitfield, see https://discord.com/channels/689445672390361176/701513924545085530/1349599402867822633
	[property: LocoStructOffset(0x0A)] Speed16 MaxSpeed,
	[property: LocoStructOffset(0x0C)] MicroZ MaxHeight,
	[property: LocoStructOffset(0x0D)] uint8_t CostIndex,
	[property: LocoStructOffset(0x0E)] int16_t BaseCostFactor,
	[property: LocoStructOffset(0x10)] int16_t HeightCostFactor,
	[property: LocoStructOffset(0x12)] int16_t SellCostFactor,
	[property: LocoStructOffset(0x14)] DatBridgeDisabledTrackFlags DisabledTrackFlags,
	[property: LocoStructOffset(0x16), Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x1A)] uint8_t CompatibleTrackObjectCount,
	[property: LocoStructOffset(0x1B), LocoStructVariableLoad, LocoArrayLength(BridgeObjectLoader.Constants.MaxNumTrackMods), LocoPropertyMaybeUnused, Browsable(false)] object_id[] TrackModHeaderIds,
	[property: LocoStructOffset(0x22)] uint8_t CompatibleRoadObjectCount,
	[property: LocoStructOffset(0x23), LocoStructVariableLoad, LocoArrayLength(BridgeObjectLoader.Constants.MaxNumRoadMods), LocoPropertyMaybeUnused, Browsable(false)] object_id[] RoadModHeaderIds,
	[property: LocoStructOffset(0x2A)] uint16_t DesignedYear
	) : ILocoStructVariableData
{
	public List<S5Header> CompatibleTrackObjects { get; set; } = [];
	public List<S5Header> CompatibleRoadObjects { get; set; } = [];

	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// compatible tracks
		CompatibleTrackObjects = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, CompatibleTrackObjectCount);
		remainingData = remainingData[(S5Header.StructLength * CompatibleTrackObjectCount)..];

		// compatible roads
		CompatibleRoadObjects = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, CompatibleRoadObjectCount);
		remainingData = remainingData[(S5Header.StructLength * CompatibleRoadObjectCount)..];

		return remainingData;
	}

	public ReadOnlySpan<byte> SaveVariable()
	{
		var headers = CompatibleTrackObjects
			.Concat(CompatibleRoadObjects);

		return headers.SelectMany(h => h.Write().ToArray()).ToArray();
	}
}
