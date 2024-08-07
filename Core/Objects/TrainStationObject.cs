
using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;
using OpenLoco.ObjectEditor.Types;

namespace OpenLoco.ObjectEditor.Objects
{
	[Flags]
	public enum TrainStationObjectFlags : uint8_t
	{
		None = 0,
		Recolourable = 1 << 0,
		NoGlass = 1 << 1,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xAE)]
	[LocoStructType(ObjectType.TrainStation)]
	[LocoStringTable("Name")]
	public record TrainStationObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02)] uint8_t DrawStyle,
		[property: LocoStructOffset(0x03)] uint8_t Height,
		[property: LocoStructOffset(0x04)] TrackTraitFlags TrackPieces,
		[property: LocoStructOffset(0x06)] int16_t BuildCostFactor,
		[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
		[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
		[property: LocoStructOffset(0x0B)] uint8_t var_0B,
		[property: LocoStructOffset(0x0C)] TrainStationObjectFlags Flags,
		[property: LocoStructOffset(0x0D)] uint8_t var_0D,
		[property: LocoStructOffset(0x0E)] image_id Image,
		[property: LocoStructOffset(0x12), LocoArrayLength(4)] uint32_t[] ImageOffsets,
		[property: LocoStructOffset(0x22)] uint8_t NumCompatible,
		[property: LocoStructOffset(0x23), LocoArrayLength(7)] uint8_t[] Mods,
		[property: LocoStructOffset(0x2A)] uint16_t DesignedYear,
		[property: LocoStructOffset(0x2C)] uint16_t ObsoleteYear,
		[property: LocoStructOffset(0x2E), LocoStructVariableLoad, LocoArrayLength(TrainStationObject.CargoOffsetBytesSize), Browsable(false)] uint8_t[] _CargoOffsetBytes,
		[property: LocoStructOffset(0x3E), LocoStructVariableLoad, LocoArrayLength(TrainStationObject.ManualPowerLength), Browsable(false)] uint8_t[] _ManualPower
	) : ILocoStruct, ILocoStructVariableData, ILocoImageTableNames
	{
		public List<S5Header> Compatible { get; set; } = [];

		public const int ManualPowerLength = 16;
		public const int CargoOffsetBytesSize = 16;
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

		public bool Validate()
		{
			if (CostIndex >= 32)
			{
				return false;
			}
			if (-SellCostFactor > BuildCostFactor)
			{
				return false;
			}
			if (BuildCostFactor <= 0)
			{
				return false;
			}
			if (DrawStyle >= 1)
			{
				return false;
			}
			return NumCompatible <= 7;
		}

		public bool TryGetImageName(int id, out string? value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		public static Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "preview_image" },
			{ 1, "preview_image_windows" },
			{ 2, "totalPreviewImages" },
		};

		// These are relative to ImageOffsets
		// ImageOffsets is the imageIds per sequenceIndex (for start/middle/end of the platform)
		//namespace Style0
		//{
		//    constexpr uint32_t straightBackNE = 0;
		//    constexpr uint32_t straightFrontNE = 1;
		//    constexpr uint32_t straightCanopyNE = 2;
		//    constexpr uint32_t straightCanopyTranslucentNE = 3;
		//    constexpr uint32_t straightBackSE = 4;
		//    constexpr uint32_t straightFrontSE = 5;
		//    constexpr uint32_t straightCanopySE = 6;
		//    constexpr uint32_t straightCanopyTranslucentSE = 7;
		//    constexpr uint32_t diagonalNE0 = 8;
		//    constexpr uint32_t diagonalNE3 = 9;
		//    constexpr uint32_t diagonalNE1 = 10;
		//    constexpr uint32_t diagonalCanopyNE1 = 11;
		//    constexpr uint32_t diagonalCanopyTranslucentNE1 = 12;
		//    constexpr uint32_t diagonalSE1 = 13;
		//    constexpr uint32_t diagonalSE2 = 14;
		//    constexpr uint32_t diagonalSE3 = 15;
		//    constexpr uint32_t diagonalCanopyTranslucentSE3 = 16;
		//    constexpr uint32_t totalNumImages = 17;
		//}
		//namespace Style1
		//{
		//    constexpr uint32_t totalNumImages = 8;
		//}
	}
}
