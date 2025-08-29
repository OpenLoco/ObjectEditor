using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.RoadStation;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.RoadObjectLoader;
using static Dat.Loaders.RoadStationObjectLoader;

namespace Dat.Loaders;

public abstract class RoadStationObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MaxImageOffsets = 4;
		public const int MaxNumCompatible = 7;
		public const int CargoOffsetBytesSize = 16;
		public const int MaxStationCargoDensity = 15;
	}

	public static class StructSizes
	{
		public const int Dat = 0x6E;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new RoadStationObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.PaintStyle = br.ReadByte();
			model.Height = br.ReadByte();
			model.RoadPieces = ((DatRoadTraitFlags)br.ReadUInt16()).Convert();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.Flags = ((DatRoadStationObjectFlags)br.ReadByte()).Convert();
			br.SkipImageId(); // Image, not part of object definition
			br.SkipImageId(Constants.MaxImageOffsets);
			var compatibleRoadObjectCount = br.ReadByte();
			br.SkipObjectId(Constants.MaxNumCompatible);
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			br.SkipObjectId(); // CargoTypeId, not part of object definition
			br.SkipByte(); // pad 0x2D
			br.SkipPointer(Constants.CargoOffsetBytesSize); // CargoOffsetBytes, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.RoadStation), null);

			// variable
			LoadVariable(br, model, compatibleRoadObjectCount);

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

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
				while (br.PeekByte(length) != LocoConstants.Terminator)
				{
					length += 4; // x, y, x, y
				}

				length += 4;
				model.CargoOffsetBytes[i][j] = br.ReadBytes(length);
			}
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (RoadStationObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.PaintStyle);
			bw.Write(model.Height);
			bw.Write((uint16_t)model.RoadPieces);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write((uint8_t)model.Flags);
			bw.WriteEmptyImageId(); // Image, not part of object definition
			bw.WriteEmptyImageId(Constants.MaxImageOffsets); // uint32_t
			bw.Write((uint8_t)model.CompatibleRoadObjects.Count);
			bw.WriteEmptyObjectId(Constants.MaxNumCompatible);
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);
			bw.WriteEmptyObjectId(); // CargoTypeId, not part of object definition
			bw.Write((uint8_t)0); // pad 0x2D
			bw.WriteEmptyPointer(Constants.CargoOffsetBytesSize); // CargoOffsetBytes, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	private static void SaveVariable(RoadStationObject model, LocoBinaryWriter bw)
	{
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
}

static class RoadStationFlagsConverter
{
	public static DatRoadStationObjectFlags Convert(this RoadStationObjectFlags roadStationObjectFlags)
		=> (DatRoadStationObjectFlags)roadStationObjectFlags;

	public static RoadStationObjectFlags Convert(this DatRoadStationObjectFlags datRoadStationObjectFlags)
		=> (RoadStationObjectFlags)datRoadStationObjectFlags;
}
