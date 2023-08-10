using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record BridgeObject(
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint8_t NoRoof,
		[property: LocoStructProperty(0x03), LocoArrayLength(0x06 - 0x03)] uint8_t[] pad_03,
		[property: LocoStructProperty(0x06)] uint16_t var_06,
		[property: LocoStructProperty(0x08)] uint8_t SpanLength,
		[property: LocoStructProperty(0x09)] uint8_t PillarSpacing,
		[property: LocoStructProperty(0x0A)] Speed16 MaxSpeed,
		[property: LocoStructProperty(0x0C)] MicroZ MaxHeight,
		[property: LocoStructProperty(0x0D)] uint8_t CostIndex,
		[property: LocoStructProperty(0x0E)] int16_t BaseCostFactor,
		[property: LocoStructProperty(0x10)] int16_t HeightCostFactor,
		[property: LocoStructProperty(0x12)] int16_t SellCostFactor,
		[property: LocoStructProperty(0x14)] uint16_t DisabledTrackCfg,
		[property: LocoStructProperty(0x16)] uint32_t Image,
		[property: LocoStructProperty(0x1A)] uint8_t TrackNumCompatible,
		[property: LocoStructProperty(0x1B), LocoArrayLength(BridgeObject.TrackModsLength)] uint8_t[] TrackMods,
		[property: LocoStructProperty(0x22)] uint8_t RoadNumCompatible,
		[property: LocoStructProperty(0x23), LocoArrayLength(BridgeObject.RoadModsLength)] uint8_t[] RoadMods,
		[property: LocoStructProperty(0x2A)] uint16_t DesignedYear
	) : ILocoStruct
	{
		public const int TrackModsLength = 7;
		public const int RoadModsLength = 7;
		public ObjectType ObjectType => ObjectType.bridge;
		public static int ObjectStructSize => 0x2C;
	}
}