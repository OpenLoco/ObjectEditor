using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x2C)]
	public record BridgeObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t NoRoof,
		[property: LocoStructOffset(0x03), LocoArrayLength(0x06 - 0x03)] uint8_t[] pad_03,
		[property: LocoStructOffset(0x06)] uint16_t var_06,
		[property: LocoStructOffset(0x08)] uint8_t SpanLength,
		[property: LocoStructOffset(0x09)] uint8_t PillarSpacing,
		[property: LocoStructOffset(0x0A)] Speed16 MaxSpeed,
		[property: LocoStructOffset(0x0C)] MicroZ MaxHeight,
		[property: LocoStructOffset(0x0D)] uint8_t CostIndex,
		[property: LocoStructOffset(0x0E)] int16_t BaseCostFactor,
		[property: LocoStructOffset(0x10)] int16_t HeightCostFactor,
		[property: LocoStructOffset(0x12)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x14)] uint16_t DisabledTrackCfg,
		[property: LocoStructOffset(0x16)] uint32_t Image,
		[property: LocoStructOffset(0x1A)] uint8_t TrackNumCompatible,
		[property: LocoStructOffset(0x1B), LocoArrayLength(BridgeObject.TrackModsLength)] uint8_t[] TrackMods,
		[property: LocoStructOffset(0x22)] uint8_t RoadNumCompatible,
		[property: LocoStructOffset(0x23), LocoArrayLength(BridgeObject.RoadModsLength)] uint8_t[] RoadMods,
		[property: LocoStructOffset(0x2A)] uint16_t DesignedYear
	) : ILocoStruct
	{
		public const int TrackModsLength = 7;
		public const int RoadModsLength = 7;
		public ObjectType ObjectType => ObjectType.bridge;
		public static int StructLength => 0x2C;
	}
}