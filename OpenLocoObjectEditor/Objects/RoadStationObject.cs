using System.ComponentModel;
using OpenLocoObjectEditor.DatFileParsing;
using OpenLocoObjectEditor.Headers;

namespace OpenLocoObjectEditor.Objects
{
	[Flags]
	public enum RoadStationFlags : uint8_t
	{
		None = 0,
		Recolourable = 1 << 0,
		Passenger = 1 << 1,
		Freight = 1 << 2,
		RoadEnd = 1 << 3,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x6E)]
	[LocoStructType(ObjectType.RoadStation)]
	[LocoStringTable("Name")]
	public class RoadStationObject(
		uint8_t paintStyle,
		uint16_t roadPieces,
		int16_t buildCostFactor,
		int16_t sellCostFactor,
		uint8_t costIndex,
		RoadStationFlags flags,
		uint8_t numCompatible,
		uint16_t designedYear,
		uint16_t obsoleteYear)
		: ILocoStruct, ILocoStructVariableData
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[LocoStructOffset(0x02)] public uint8_t PaintStyle { get; set; } = paintStyle;
		//[LocoStructOffset(0x03)] uint8_t pad_03,
		[LocoStructOffset(0x04)] public uint16_t RoadPieces { get; set; } = roadPieces;
		[LocoStructOffset(0x06)] public int16_t BuildCostFactor { get; set; } = buildCostFactor;
		[LocoStructOffset(0x08)] public int16_t SellCostFactor { get; set; } = sellCostFactor;
		[LocoStructOffset(0x0A)] public uint8_t CostIndex { get; set; } = costIndex;
		[LocoStructOffset(0x0B)] public RoadStationFlags Flags { get; set; } = flags;
		//[LocoStructOffset(0x0C)] image_id Image,
		//[LocoStructOffset(0x10), LocoArrayLength(4)] public uint32_t[] ImageOffsets { get; set; }
		[LocoStructOffset(0x20)] public uint8_t NumCompatible { get; set; } = numCompatible;
		//[LocoStructOffset(0x21), LocoArrayLength(7)] uint8_t[] Mods,
		[LocoStructOffset(0x28)] public uint16_t DesignedYear { get; set; } = designedYear;
		[LocoStructOffset(0x2A)] public uint16_t ObsoleteYear { get; set; } = obsoleteYear;
		//[LocoStructOffset(0x2C)] object_index CargoType
		//[LocoStructOffset(0x2D)] uint8_t pad_2D
		//[LocoStructProperty(0x2E)] uint8_t CargoOffsetBytes[4][4]

		public List<S5Header> Compatible { get; set; } = [];

		public S5Header CargoType { get; set; }

		public uint8_t[][][] CargoOffsetBytes { get; set; }

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// compatible
			Compatible = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, NumCompatible);
			remainingData = remainingData[(S5Header.StructLength * NumCompatible)..];

			// cargo
			if (Flags.HasFlag(RoadStationFlags.Passenger) || Flags.HasFlag(RoadStationFlags.Freight))
			{
				CargoType = S5Header.Read(remainingData[..S5Header.StructLength]);
				remainingData = remainingData[(S5Header.StructLength * 1)..];
			}

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

				// cargo
				if (Flags.HasFlag(RoadStationFlags.Passenger) || Flags.HasFlag(RoadStationFlags.Freight))
				{
					ms.Write(CargoType.Write());
				}

				// cargo offsets
				for (var i = 0; i < 4; ++i)
				{
					for (var j = 0; j < 4; ++j)
					{
						ms.Write(CargoOffsetBytes[i][j]);
					}
				}

				return ms.ToArray();
			}
		}
	}
}
