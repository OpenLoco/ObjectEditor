
using System.ComponentModel;
using OpenLocoObjectEditor.Data;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;

namespace OpenLocoObjectEditor.Objects
{
	[Flags]
	public enum TrainStationObjectFlags : uint8_t
	{
		None = 0,
		Recolourable = 1 << 0,
		unk1 = 1 << 1, // Has no canopy??
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xAE)]
	[LocoStructType(ObjectType.TrainStation)]
	[LocoStringTable("Name")]
	public class TrainStationObject(
		uint8_t paintStyle,
		uint8_t var_03,
		uint16_t trackPieces,
		int16_t buildCostFactor,
		int16_t sellCostFactor,
		uint8_t costIndex,
		uint8_t var_0B,
		TrainStationObjectFlags flags,
		uint8_t var_0D,
		uint8_t numCompatible,
		uint16_t designedYear,
		uint16_t obsoleteYear)
		: ILocoStruct, ILocoStructVariableData
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[LocoStructOffset(0x02)] public uint8_t PaintStyle { get; set; } = paintStyle;
		[LocoStructOffset(0x03)] public uint8_t var_03 { get; set; } = var_03;
		[LocoStructOffset(0x04)] public uint16_t TrackPieces { get; set; } = trackPieces;
		[LocoStructOffset(0x06)] public int16_t BuildCostFactor { get; set; } = buildCostFactor;
		[LocoStructOffset(0x08)] public int16_t SellCostFactor { get; set; } = sellCostFactor;
		[LocoStructOffset(0x0A)] public uint8_t CostIndex { get; set; } = costIndex;
		[LocoStructOffset(0x0B)] public uint8_t var_0B { get; set; } = var_0B;
		[LocoStructOffset(0x0C)] public TrainStationObjectFlags Flags { get; set; } = flags;
		[LocoStructOffset(0x0D)] public uint8_t var_0D { get; set; } = var_0D;
		//[LocoStructOffset(0x0E)] image_id Image,
		//[LocoStructOffset(0x12), LocoArrayLength(4)] public uint32_t[] ImageOffsets { get; set; }
		[LocoStructOffset(0x22)] public uint8_t NumCompatible { get; set; } = numCompatible;
		//[LocoStructOffset(0x23), LocoArrayLength(7)] uint8_t[] Mods,
		[LocoStructOffset(0x2A)] public uint16_t DesignedYear { get; set; } = designedYear;
		[LocoStructOffset(0x2C)] public uint16_t ObsoleteYear { get; set; } = obsoleteYear;
		//[LocoStructProperty(0x2E)] const std::byte* CargoOffsetBytes[4][4]
		//[LocoStructProperty(0x??)] const std::byte* ManualPower[ManualPowerLength]

		public List<S5Header> Compatible { get; set; } = [];

		public const int ManualPowerLength = 16;

		public uint8_t[][][] CargoOffsetBytes { get; set; }

		public uint8_t[][] ManualPower { get; set; }

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// compatible
			Compatible = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumCompatible);
			remainingData = remainingData[(S5Header.StructLength * NumCompatible)..];

			// cargo offsets (for drawing the cargo on the station)
			CargoOffsetBytes = new byte[4][][];
			for (var i = 0; i < 4; ++i)
			{
				CargoOffsetBytes[i] = new byte[4][];
				for (var j = 0; j < 4; ++j)
				{
					var bytes = 0;
					bytes++;
					var length = 1;

					while (remainingData[bytes] != 0xFF)
					{
						length += 4; // x, y, x, y
						bytes += 4;
					}

					length += 4;
					CargoOffsetBytes[i][j] = remainingData[..length].ToArray();
					remainingData = remainingData[length..];
				}
			}

			// very similar to cargoffsetbytes
			ManualPower = new byte[ManualPowerLength][];
			for (var i = 0; i < ManualPowerLength; ++i)
			{
				var bytes = 0;
				bytes++;
				var length = 1;

				while (remainingData[bytes] != 0xFF)
				{
					length += 4; // x, y, x, y
					bytes += 4;
				}

				length += 4;
				ManualPower[i] = remainingData[..length].ToArray();
				remainingData = remainingData[length..];
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> Save()
		{
			using (var ms = new MemoryStream())
			{
				// compatible
				foreach (var co in Compatible)
				{
					ms.Write(co.Write());
				}

				// cargo offsets
				for (var i = 0; i < 4; ++i)
				{
					for (var j = 0; j < 4; ++j)
					{
						ms.Write(CargoOffsetBytes[i][j]);
					}
				}

				// manual power offsets
				for (var i = 0; i < ManualPowerLength; ++i)
				{
					ms.Write(ManualPower[i]);
				}

				return ms.ToArray();
			}
		}
	}
}
