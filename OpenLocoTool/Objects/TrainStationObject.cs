
using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	[Flags]
	public enum TrainStationFlags : uint8_t
	{
		None = 0,
		Recolourable = 1 << 0,
		unk1 = 1 << 1, // Has no canopy??
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xAE)]
	public record TrainStationObject(
		[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t PaintStyle,
		[property: LocoStructOffset(0x03)] uint8_t var_03,
		[property: LocoStructOffset(0x04)] uint16_t TrackPieces,
		[property: LocoStructOffset(0x06)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
		[property: LocoStructOffset(0x0B)] uint8_t var_0B,
		[property: LocoStructOffset(0x0C)] RoadStationFlags Flags,
		[property: LocoStructOffset(0x0D)] uint8_t var_0D,
		[property: LocoStructOffset(0x0E)] uint32_t Image,
		[property: LocoStructOffset(0x12), LocoArrayLength(4)] uint32_t[] var_12,
		[property: LocoStructOffset(0x22)] uint8_t NumCompatible,
		[property: LocoStructOffset(0x23), LocoArrayLength(7)] uint8_t[] Mods,
		[property: LocoStructOffset(0x2A)] uint16_t DesignedYear,
		[property: LocoStructOffset(0x2C)] uint16_t ObsoleteYear
		//[property: LocoStructProperty(0x2E)] const std::byte* CargoOffsetBytes[4][4]
		//[property: LocoStructProperty(0x??)] const std::byte* var_6E[Var6ELength]
		) : ILocoStruct, ILocoStructVariableData
	{
		public static ObjectType ObjectType => ObjectType.TrainStation;
		public static int StructSize => 0xAE;

		public const int Var6ELength = 16;

		uint8_t[,] CargoOffsetBytes { get; set; }
		uint8_t[] var_6E { get; set; }

		public ReadOnlySpan<byte> Load(ReadOnlySpan<byte> remainingData)
		{
			// compatible roads/tracks
			remainingData = remainingData[(S5Header.StructLength * NumCompatible)..];

			// cargo offsets (for drawing the cargo on the station) (same as roadstation code)
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

			// very similar to cargoffsetbytes
			var_6E = new byte[Var6ELength];
			for (var i = 0; i < Var6ELength; ++i)
			{
				var_6E[i] = remainingData[0];
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

			return remainingData;
		}

		public ReadOnlySpan<byte> Save() => throw new NotImplementedException();
	}
}
