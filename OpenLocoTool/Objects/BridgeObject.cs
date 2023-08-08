using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record BridgeObject(
		[property: LocoStructProperty] string_id Name,
		[property: LocoStructProperty] uint8_t NoRoof, // 0x02
		[property: LocoStructProperty, LocoArrayLength(0x06 - 0x03)] uint8_t pad_03,
		[property: LocoStructProperty] uint16_t var_06,            // 0x06
		[property: LocoStructProperty] uint8_t SpanLength,         // 0x08
		[property: LocoStructProperty] uint8_t PillarSpacing,      // 0x09
		[property: LocoStructProperty] Speed16 MaxSpeed,           // 0x0A
		[property: LocoStructProperty] MicroZ MaxHeight,    // 0x0C MicroZ!
		[property: LocoStructProperty] uint8_t CostIndex,          // 0x0D
		[property: LocoStructProperty] int16_t BaseCostFactor,     // 0x0E
		[property: LocoStructProperty] int16_t HeightCostFactor,   // 0x10
		[property: LocoStructProperty] int16_t SellCostFactor,     // 0x12
		[property: LocoStructProperty] uint16_t DisabledTrackCfg,  // 0x14
		[property: LocoStructProperty] uint32_t Image,             // 0x16
		[property: LocoStructProperty] uint8_t TrackNumCompatible, // 0x1A
		[property: LocoStructProperty, LocoArrayLength(7)] uint8_t TrackMods,       // 0x1B
		[property: LocoStructProperty] uint8_t RoadNumCompatible,  // 0x22
		[property: LocoStructProperty, LocoArrayLength(7)] uint8_t RoadMods,        // 0x23
		[property: LocoStructProperty] uint16_t DesignedYear      // 0x2A
	) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.bridge;
		public int ObjectStructSize => 0x2C;
		public static ILocoStruct Read(ReadOnlySpan<byte> data) => throw new NotImplementedException();
		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}