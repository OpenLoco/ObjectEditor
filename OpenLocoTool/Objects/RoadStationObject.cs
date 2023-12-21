using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
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
	public record RoadStationObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t PaintStyle,
		[property: LocoStructOffset(0x03)] uint8_t pad_03,
		[property: LocoStructOffset(0x04)] uint16_t RoadPieces,
		[property: LocoStructOffset(0x06)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
		[property: LocoStructOffset(0x0B)] RoadStationFlags Flags,
		[property: LocoStructOffset(0x0C)] uint32_t Image,
		[property: LocoStructOffset(0x10), LocoArrayLength(4)] uint32_t[] var_10,
		[property: LocoStructOffset(0x20)] uint8_t NumCompatible,
		[property: LocoStructOffset(0x21), LocoArrayLength(7)] uint8_t[] Mods,
		[property: LocoStructOffset(0x28)] uint16_t DesignedYear,
		[property: LocoStructOffset(0x2A)] uint16_t ObsoleteYear,
		[property: LocoStructOffset(0x2C)] uint8_t CargoType,
		[property: LocoStructOffset(0x2D)] uint8_t pad_2D
		//[property: LocoStructProperty(0x2E)] uint8_t CargoOffsetBytes[4][4]
		) : ILocoStruct, ILocoStructVariableData
	{
		uint8_t[,] CargoOffsetBytes { get; set; }

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// compatible roads/tracks
			remainingData = remainingData[(S5Header.StructLength * NumCompatible)..];

			// cargo
			if (Flags.HasFlag(RoadStationFlags.Passenger) || Flags.HasFlag(RoadStationFlags.Freight))
			{
				remainingData = remainingData[(S5Header.StructLength * 1)..];
			}

			// cargo offsets (for drawing the cargo on the station)
			CargoOffsetBytes = new byte[4, 4];
			for (var i = 0; i < 4; ++i)
			{
				for (var j = 0; j < 4; ++j)
				{
					CargoOffsetBytes[i, j] = remainingData[0];
					var bytes = 0;
					bytes++;
					var length = 1;

					while (remainingData[bytes] != 0xFF)
					{
						length += 4; // x, y, x, y
						bytes += 4;
					}

					length += 4;
					remainingData = remainingData[length..];
				}
			}

			return remainingData;
		}

		public ReadOnlySpan<byte> Save() => throw new NotImplementedException();
	}
}
