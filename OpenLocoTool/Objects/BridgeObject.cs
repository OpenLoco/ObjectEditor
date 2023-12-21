using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x2C)]
	[LocoStructType(ObjectType.Bridge)]
	public record BridgeObject_(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
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
		[property: LocoStructOffset(0x2A)] uint16_t DesignedYear) : ILocoStruct, ILocoStructVariableData
	{
		public const int TrackModsLength = 7;
		public const int RoadModsLength = 7;

		// return number of bytes read
		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			var bytesRead = (TrackNumCompatible + RoadNumCompatible) * S5Header.StructLength;
			return remainingData[bytesRead..];
		}

		public ReadOnlySpan<byte> Save() => throw new NotImplementedException();
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x2C)]
	[LocoStructType(ObjectType.Bridge)]
	[LocoStringTable("Name")]
	public class BridgeObject : ILocoStruct
	{
		public uint8_t NoRoof { get; set; }
		public uint16_t var_06 { get; set; }
		public uint8_t SpanLength { get; set; }
		public uint8_t PillarSpacing { get; set; }
		public Speed16 MaxSpeed { get; set; }
		public MicroZ MaxHeight { get; set; }
		public uint8_t CostIndex { get; set; }
		public int16_t BaseCostFactor { get; set; }
		public int16_t HeightCostFactor { get; set; }
		public int16_t SellCostFactor { get; set; }
		public uint16_t DisabledTrackCfg { get; set; }
		public uint8_t TrackNumCompatible { get; set; }
		public uint8_t RoadNumCompatible { get; set; }
		public uint16_t DesignedYear { get; set; }

		public const int TrackModsLength = 7;
		public const int RoadModsLength = 7;
	}
}