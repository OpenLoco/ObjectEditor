using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Road;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class RoadStationObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MaxImageOffsets = 4;
		public const int MaxNumCompatible = 7;
		public const int CargoOffsetBytesSize = 16;
	}

	public static class StructSizes
	{
		public const int Dat = 0x6E;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new RoadStationObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId(); // Name offset, not part of object definition
			model.PaintStyle = br.ReadByte();
			model.Height = br.ReadByte();
			model.RoadPieces = (RoadTraitFlags)br.ReadUInt16();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.Flags = (RoadStationObjectFlags)br.ReadByte();
			_ = br.SkipImageId(); // Image, not part of object definition
			_ = br.SkipImageId(Constants.MaxImageOffsets);
			var compatibleRoadObjectCount = br.ReadByte();
			_ = br.SkipObjectId(Constants.MaxNumCompatible);
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			_ = br.SkipObjectId(); // CargoTypeId, not part of object definition
			_ = br.SkipByte(); // pad 0x2D
			_ = br.SkipPointer(Constants.CargoOffsetBytesSize); // CargoOffsetBytes, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.RoadStation), null);

			// variable
			LoadVariable(br, model, compatibleRoadObjectCount);

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.RoadStation, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, RoadStationObject model, byte compatibleRoadObjectCount)
	{
		// compatible road objects
		model.CompatibleRoadObjects = br.ReadS5HeaderList(compatibleRoadObjectCount);

		// cargo
		if (model.Flags.HasFlag(RoadStationObjectFlags.Passenger) || model.Flags.HasFlag(RoadStationObjectFlags.Freight))
		{
			model.CargoType = br.ReadS5Header();
		}

		// cargo offsets
		model.CargoOffsetBytes = new byte[4][][];
		for (var i = 0; i < 4; ++i)
		{
			model.CargoOffsetBytes[i] = new byte[4][];
			for (var j = 0; j < 4; ++j)
			{
				var length = 1;
				while (br.PeekByte(length) != 0xFF)
				{
					length += 4; // x, y, x, y
				}

				length += 4;
				model.CargoOffsetBytes[i][j] = br.ReadBytes(length);
			}
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (RoadStationObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition
			bw.Write(model.PaintStyle);
			bw.Write(model.Height);
			bw.Write((uint16_t)model.RoadPieces);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write((uint8_t)model.Flags);
			bw.WriteImageId(); // Image, not part of object definition
			bw.WriteImageId(Constants.MaxImageOffsets); // uint32_t
			bw.Write((uint8_t)model.CompatibleRoadObjects.Count);
			bw.WriteObjectId(Constants.MaxNumCompatible);
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);
			bw.WriteObjectId(); // CargoTypeId, not part of object definition
			bw.Write((uint8_t)0); // pad 0x2D
			bw.WritePointer(Constants.CargoOffsetBytesSize); // CargoOffsetBytes, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable

			// compatible road objects
			bw.WriteS5HeaderList(model.CompatibleRoadObjects);

			// cargo
			if (model.Flags.HasFlag(RoadStationObjectFlags.Passenger) || model.Flags.HasFlag(RoadStationObjectFlags.Freight))
			{
				ArgumentNullException.ThrowIfNull(model.CargoType, nameof(model.CargoType));
				bw.WriteS5Header(model.CargoType);
			}

			// cargo offsets
			for (var i = 0; i < 4; ++i)
			{
				for (var j = 0; j < 4; ++j)
				{
					bw.Write(model.CargoOffsetBytes[i][j]);
				}
			}

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}
}

[Flags]
internal enum DatRoadStationObjectFlags : uint8_t
{
	None = 0,
	Recolourable = 1 << 0,
	Passenger = 1 << 1,
	Freight = 1 << 2,
	RoadEnd = 1 << 3,
}

[LocoStructSize(0x6E)]
[LocoStructType(DatObjectType.RoadStation)]
internal record DatRoadStationObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint8_t PaintStyle,
	[property: LocoStructOffset(0x03)] uint8_t Height,
	[property: LocoStructOffset(0x04)] DatRoadTraitFlags RoadPieces,
	[property: LocoStructOffset(0x06)] int16_t BuildCostFactor,
	[property: LocoStructOffset(0x08)] int16_t SellCostFactor,
	[property: LocoStructOffset(0x0A)] uint8_t CostIndex,
	[property: LocoStructOffset(0x0B)] DatRoadStationObjectFlags Flags,
	[property: LocoStructOffset(0x0C), LocoStructVariableLoad, Browsable(false)] image_id Image,
	[property: LocoStructOffset(0x10), LocoStructVariableLoad, LocoArrayLength(RoadStationObjectLoader.Constants.MaxImageOffsets)] uint32_t[] ImageOffsets,
	[property: LocoStructOffset(0x20)] uint8_t CompatibleRoadObjectCount,
	[property: LocoStructOffset(0x21), LocoStructVariableLoad, LocoArrayLength(RoadStationObjectLoader.Constants.MaxNumCompatible)] object_id[] _Compatible,
	[property: LocoStructOffset(0x28)] uint16_t DesignedYear,
	[property: LocoStructOffset(0x2A)] uint16_t ObsoleteYear,
	[property: LocoStructOffset(0x2C), LocoStructVariableLoad, Browsable(false)] object_id _CargoTypeId,
	[property: LocoStructOffset(0x2D), Browsable(false)] uint8_t pad_2D,
	[property: LocoStructOffset(0x2E), LocoStructVariableLoad, LocoArrayLength(RoadStationObjectLoader.Constants.CargoOffsetBytesSize), Browsable(false)] uint8_t[] _CargoOffsetBytes
)
{
	public List<S5Header> CompatibleRoadObjects { get; set; } = [];

	public S5Header CargoType { get; set; }

	public uint8_t[][][] CargoOffsetBytes { get; set; }

	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// compatible
		CompatibleRoadObjects = SawyerStreamReader.ReadS5HeaderList(remainingData, CompatibleRoadObjectCount);
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
}
