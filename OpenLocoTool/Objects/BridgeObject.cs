using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x2C)]
	public class BridgeObject : ILocoStruct, ILocoStructVariableData
	{
		public const ObjectType ObjType = ObjectType.Bridge;
		public const int StructSize = 0x2C;

		public const int TrackModsLength = 7;
		public const int RoadModsLength = 7;

		public BridgeObject(ushort name, byte noRoof, byte[] pad_03, ushort var_06, byte spanLength, byte pillarSpacing, short maxSpeed, byte maxHeight, byte costIndex, short baseCostFactor, short heightCostFactor, short sellCostFactor, ushort disabledTrackCfg, uint image, byte trackNumCompatible, byte[] trackMods, byte roadNumCompatible, byte[] roadMods, ushort designedYear)
		{
			Name = name;
			NoRoof = noRoof;
			this.pad_03 = pad_03;
			this.var_06 = var_06;
			SpanLength = spanLength;
			PillarSpacing = pillarSpacing;
			MaxSpeed = maxSpeed;
			MaxHeight = maxHeight;
			CostIndex = costIndex;
			BaseCostFactor = baseCostFactor;
			HeightCostFactor = heightCostFactor;
			SellCostFactor = sellCostFactor;
			DisabledTrackCfg = disabledTrackCfg;
			Image = image;
			TrackNumCompatible = trackNumCompatible;
			TrackMods = trackMods;
			RoadNumCompatible = roadNumCompatible;
			RoadMods = roadMods;
			DesignedYear = designedYear;
		}

		// return number of bytes read
		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			var bytesRead = (TrackNumCompatible + RoadNumCompatible) * S5Header.StructLength;
			return remainingData[bytesRead..];
		}

		[LocoStructOffset(0x00)] public string_id Name { get; set; }
		[LocoStructOffset(0x02)] public uint8_t NoRoof { get; set; }
		[LocoStructOffset(0x03), LocoArrayLength(0x06 - 0x03)] public uint8_t[] pad_03 { get; set; }
		[LocoStructOffset(0x06)] public uint16_t var_06 { get; set; }
		[LocoStructOffset(0x08)] public uint8_t SpanLength { get; set; }
		[LocoStructOffset(0x09)] public uint8_t PillarSpacing { get; set; }
		[LocoStructOffset(0x0A)] public Speed16 MaxSpeed { get; set; }
		[LocoStructOffset(0x0C)] public MicroZ MaxHeight { get; set; }
		[LocoStructOffset(0x0D)] public uint8_t CostIndex { get; set; }
		[LocoStructOffset(0x0E)] public int16_t BaseCostFactor { get; set; }
		[LocoStructOffset(0x10)] public int16_t HeightCostFactor { get; set; }
		[LocoStructOffset(0x12)] public int16_t SellCostFactor { get; set; }
		[LocoStructOffset(0x14)] public uint16_t DisabledTrackCfg { get; set; }
		[LocoStructOffset(0x16)] public uint32_t Image { get; set; }
		[LocoStructOffset(0x1A)] public uint8_t TrackNumCompatible { get; set; }
		[LocoStructOffset(0x1B), LocoArrayLength(BridgeObject.TrackModsLength)] public uint8_t[] TrackMods { get; set; }
		[LocoStructOffset(0x22)] public uint8_t RoadNumCompatible { get; set; }
		[LocoStructOffset(0x23), LocoArrayLength(BridgeObject.RoadModsLength)] public uint8_t[] RoadMods { get; set; }
		[LocoStructOffset(0x2A)] public uint16_t DesignedYear { get; set; }
	}

}