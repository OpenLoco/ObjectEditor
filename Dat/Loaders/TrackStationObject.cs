using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.TrackStation;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class TrackStationObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MaxImageOffsets = 4;
		public const int MaxNumCompatible = 7;
		public const int ManualPowerLength = 16;
		public const int CargoOffsetBytesSize = 16;
		public const int MaxStationCargoDensity = 15;
	}

	public static class Sizes
	{
		public const int CargoOffset = 6;
	}

	public static LocoObject Load(MemoryStream stream) => throw new NotImplementedException();
	public static void Save(MemoryStream ms, LocoObject obj) => throw new NotImplementedException();
}

[Flags]
internal enum DatTrackTraitFlags : uint16_t
{
	None = 0,
	Diagonal = 1 << 0,
	LargeCurve = 1 << 1,
	NormalCurve = 1 << 2,
	SmallCurve = 1 << 3,
	VerySmallCurve = 1 << 4,
	Slope = 1 << 5,
	SteepSlope = 1 << 6,
	OneSided = 1 << 7,
	SlopedCurve = 1 << 8,
	SBend = 1 << 9,
	Junction = 1 << 10,
}

[Flags]
internal enum DatTrackStationObjectFlags : uint8_t
{
	None = 0,
	Recolourable = 1 << 0,
	NoGlass = 1 << 1,
}

[LocoStructSize(0xAE)]
[LocoStructType(DatObjectType.TrackStation)]
internal record DatTrackStationObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint8_t PaintStyle,
	[property: LocoStructOffset(0x03)] uint8_t Height,
	[property: LocoStructOffset(0x04)] DatTrackTraitFlags TrackPieces,
	[property: LocoStructOffset(0x06)] int16_t BuildCostFactor,
	[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
	[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
	[property: LocoStructOffset(0x0B)] uint8_t var_0B,
	[property: LocoStructOffset(0x0C)] DatTrackStationObjectFlags Flags,
	[property: LocoStructOffset(0x0D)] uint8_t var_0D,
	[property: LocoStructOffset(0x0E), LocoStructVariableLoad, Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x12), LocoArrayLength(TrackStationObjectLoader.Constants.MaxImageOffsets), Browsable(false)] uint32_t[] ImageOffsets,
	[property: LocoStructOffset(0x22)] uint8_t CompatibleTrackObjectCount,
	[property: LocoStructOffset(0x23), LocoArrayLength(TrackStationObjectLoader.Constants.MaxNumCompatible), Browsable(false)] object_id[] _Compatible, // only used for runtime loco, this isn't part of object 'definition'
	[property: LocoStructOffset(0x2A)] uint16_t DesignedYear,
	[property: LocoStructOffset(0x2C)] uint16_t ObsoleteYear,
	[property: LocoStructOffset(0x2E), LocoStructVariableLoad, LocoArrayLength(TrackStationObjectLoader.Constants.CargoOffsetBytesSize), Browsable(false)] uint8_t[] _CargoOffsetBytes,
	[property: LocoStructOffset(0x3E), LocoStructVariableLoad, LocoArrayLength(TrackStationObjectLoader.Constants.ManualPowerLength), Browsable(false)] uint8_t[] _ManualPower
)
{
	public List<S5Header> CompatibleTrackObjects { get; set; } = [];

	public uint8_t[][][] CargoOffsetBytes { get; set; }

	public uint8_t[][] ManualPower { get; set; }

	public CargoOffset[] CargoOffsets { get; init; } = [.. Enumerable.Repeat(new CargoOffset(), 15)];

	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// compatible
		CompatibleTrackObjects = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, CompatibleTrackObjectCount);
		remainingData = remainingData[(S5Header.StructLength * CompatibleTrackObjectCount)..];

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

		// very similar to cargo offset bytes
		ManualPower = new byte[TrackStationObjectLoader.Constants.ManualPowerLength][];
		for (var i = 0; i < TrackStationObjectLoader.Constants.ManualPowerLength; ++i)
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

	public ReadOnlySpan<byte> SaveVariable()
	{
		using (var ms = new MemoryStream())
		{
			// compatible track objects
			foreach (var co in CompatibleTrackObjects)
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
			for (var i = 0; i < TrackStationObjectLoader.Constants.ManualPowerLength; ++i)
			{
				ms.Write(ManualPower[i]);
			}

			return ms.ToArray();
		}
	}
}
