using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using System.ComponentModel;

namespace Dat.Objects;

[Flags]
public enum DatRoadStationObjectFlags : uint8_t
{
	None = 0,
	Recolourable = 1 << 0,
	Passenger = 1 << 1,
	Freight = 1 << 2,
	RoadEnd = 1 << 3,
}

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x6E)]
[LocoStructType(DatObjectType.RoadStation)]
public record RoadStationObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint8_t PaintStyle,
	[property: LocoStructOffset(0x03)] uint8_t Height,
	[property: LocoStructOffset(0x04)] DatRoadTraitFlags RoadPieces,
	[property: LocoStructOffset(0x06)] int16_t BuildCostFactor,
	[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
	[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
	[property: LocoStructOffset(0x0B)] DatRoadStationObjectFlags Flags,
	[property: LocoStructOffset(0x0C), LocoStructVariableLoad, Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x10), LocoStructVariableLoad, LocoArrayLength(RoadStationObject.MaxImageOffsets)] uint32_t[] ImageOffsets,
	[property: LocoStructOffset(0x20)] uint8_t CompatibleRoadObjectCount,
	[property: LocoStructOffset(0x21), LocoStructVariableLoad, LocoArrayLength(RoadStationObject.MaxNumCompatible)] object_id[] _Compatible,
	[property: LocoStructOffset(0x28)] uint16_t DesignedYear,
	[property: LocoStructOffset(0x2A)] uint16_t ObsoleteYear,
	[property: LocoStructOffset(0x2C), LocoStructVariableLoad, Browsable(false)] object_id _CargoTypeId,
	[property: LocoStructOffset(0x2D), Browsable(false)] uint8_t pad_2D,
	[property: LocoStructOffset(0x2E), LocoStructVariableLoad, LocoArrayLength(RoadStationObject.CargoOffsetBytesSize), Browsable(false)] uint8_t[] _CargoOffsetBytes
) : ILocoStruct, ILocoStructVariableData, IImageTableNameProvider
{
	public const int MaxImageOffsets = 4;
	public const int MaxNumCompatible = 7;
	public const int CargoOffsetBytesSize = 16;
	public List<S5Header> CompatibleRoadObjects { get; set; } = [];

	public S5Header CargoType { get; set; }

	public uint8_t[][][] CargoOffsetBytes { get; set; }

	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// compatible
		CompatibleRoadObjects = SawyerStreamReader.LoadVariableCountS5Headers(remainingData, CompatibleRoadObjectCount);
		remainingData = remainingData[(S5Header.StructLength * CompatibleRoadObjectCount)..];

		// cargo
		if (Flags.HasFlag(DatRoadStationObjectFlags.Passenger) || Flags.HasFlag(DatRoadStationObjectFlags.Freight))
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

	public ReadOnlySpan<byte> SaveVariable()
	{
		using (var ms = new MemoryStream())
		{
			// compatible
			foreach (var co in CompatibleRoadObjects)
			{
				ms.Write(co.Write());
			}

			// cargo
			if (Flags.HasFlag(DatRoadStationObjectFlags.Passenger) || Flags.HasFlag(DatRoadStationObjectFlags.Freight))
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

		if (PaintStyle >= 1)
		{
			return false;
		}

		if (CompatibleRoadObjectCount > 7)
		{
			return false;
		}

		if (Flags.HasFlag(DatRoadStationObjectFlags.Passenger) && Flags.HasFlag(DatRoadStationObjectFlags.Freight))
		{
			return false;
		}

		return true;
	}

	public bool TryGetImageName(int id, out string? value)
		=> ImageIdNameMap.TryGetValue(id, out value);

	public static Dictionary<int, string> ImageIdNameMap = new()
	{
		{ 0, "preview_image" },
		{ 1, "preview_image_windows" },
		{ 2, "totalPreviewImages" },

		// These are relative to ImageOffsets
		// ImageOffsets is the imageIds per sequenceIndex (for start/middle/end of the platform)
		//namespace Style0
		//{
		//	constexpr uint32_t totalNumImages = 8;
		//}
};
}
