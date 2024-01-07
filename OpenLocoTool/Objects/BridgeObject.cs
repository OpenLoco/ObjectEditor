using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x2C)]
	[LocoStructType(ObjectType.Bridge)]
	[LocoStringTable("Name")]
	public class BridgeObject(
		uint8_t noRoof,
		uint16_t var_06,
		uint8_t spanLength,
		uint8_t pillarSpacing,
		Speed16 maxSpeed,
		MicroZ maxHeight,
		uint8_t costIndex,
		int16_t baseCostFactor,
		int16_t heightCostFactor,
		int16_t sellCostFactor,
		uint16_t disabledTrackCfg,
		uint8_t trackNumCompatible,
		uint8_t roadNumCompatible,
		uint16_t designedYear)
		: ILocoStruct, ILocoStructVariableData
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name { get; set; }
		[LocoStructOffset(0x02)] public uint8_t NoRoof { get; set; } = noRoof;
		//[LocoStructOffset(0x03)] uint8_t[] pad_03 { get; set; } = new uint8_t[3];
		[LocoStructOffset(0x06)] public uint16_t var_06 { get; set; } = var_06;
		[LocoStructOffset(0x08)] public uint8_t SpanLength { get; set; } = spanLength;
		[LocoStructOffset(0x09)] public uint8_t PillarSpacing { get; set; } = pillarSpacing;
		[LocoStructOffset(0x0A)] public Speed16 MaxSpeed { get; set; } = maxSpeed;
		[LocoStructOffset(0x0C)] public MicroZ MaxHeight { get; set; } = maxHeight;
		[LocoStructOffset(0x0D)] public uint8_t CostIndex { get; set; } = costIndex;
		[LocoStructOffset(0x0E)] public int16_t BaseCostFactor { get; set; } = baseCostFactor;
		[LocoStructOffset(0x10)] public int16_t HeightCostFactor { get; set; } = heightCostFactor;
		[LocoStructOffset(0x12)] public int16_t SellCostFactor { get; set; } = sellCostFactor;
		[LocoStructOffset(0x14)] public uint16_t DisabledTrackCfg { get; set; } = disabledTrackCfg;
		//[LocoStructOffset(0x16)] image_id Image { get; set; }
		[LocoStructOffset(0x1A)] public uint8_t TrackNumCompatible { get; set; } = trackNumCompatible;
		//[LocoStructOffset(0x1B)] uint8_t[] trackMods { get; set; } = new uint8_t[7];
		[LocoStructOffset(0x22)] public uint8_t RoadNumCompatible { get; set; } = roadNumCompatible;
		//[LocoStructOffset(0x23)] uint8_t[] roadMods { get; set; } = new uint8_t[7];
		[LocoStructOffset(0x2A)] public uint16_t DesignedYear { get; set; } = designedYear;

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			var bytesRead = (TrackNumCompatible + RoadNumCompatible) * S5Header.StructLength;
			return remainingData[bytesRead..];
		}

		public ReadOnlySpan<byte> Save()
		{
			var variableDataSize = (TrackNumCompatible + RoadNumCompatible) * S5Header.StructLength;
			return new byte[variableDataSize];
		}
	}
}