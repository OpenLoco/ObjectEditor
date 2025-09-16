using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class TrackStationObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MaxImageOffsets = 4;
		public const int MaxNumCompatible = 7;
		public const int var_6E_Length = 16;
		public const int CargoOffsetBytesSize = 16;
		public const int MaxStationCargoDensity = 15;
	}

	public static class StructSizes
	{
		public const int Dat = 0xAE;
		public const int CargoOffset = 6;
	}

	public static ObjectType ObjectType => ObjectType.TrackStation;
	public static DatObjectType DatObjectType => DatObjectType.TrackStation;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new TrackStationObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.PaintStyle = br.ReadByte();
			model.Height = br.ReadByte();
			model.TrackPieces = (TrackTraitFlags)br.ReadUInt16();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.var_0B = br.ReadByte();
			model.Flags = (TrackStationObjectFlags)br.ReadByte();
			model.var_0D = br.ReadByte();
			br.SkipImageId(); // Image, not part of object definition
			br.SkipImageId(Constants.MaxImageOffsets);
			var compatibleTrackObjectCount = br.ReadByte();
			br.SkipObjectId(Constants.MaxNumCompatible);
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			br.SkipPointer(Constants.CargoOffsetBytesSize); // CargoOffsetBytes, not part of object definition
			br.SkipPointer(Constants.var_6E_Length); // CargoOffsetBytes, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			LoadVariable(br, model, compatibleTrackObjectCount);

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableLoader.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, TrackStationObject model, byte compatibleRoadObjectCount)
	{
		// compatible road objects
		model.CompatibleTrackObjects = br.ReadS5HeaderList(compatibleRoadObjectCount);

		// cargo offsets
		model.CargoOffsets = br.ReadCargoOffsets();

		// very similar to cargo offset bytes
		model.var_6E = new byte[Constants.var_6E_Length][];
		for (var i = 0; i < Constants.var_6E_Length; ++i)
		{
			var length = 1;
			while (br.PeekByte(length) != LocoConstants.Terminator)
			{
				length += 4; // x, y, x, y
			}

			length += 4;
			model.var_6E[i] = br.ReadBytes(length);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (TrackStationObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.PaintStyle);
			bw.Write(model.Height);
			bw.Write((uint16_t)model.TrackPieces);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write(model.var_0B);
			bw.Write((uint8_t)model.Flags);
			bw.Write(model.var_0D);
			bw.WriteEmptyImageId(); // Image, not part of object definition
			bw.WriteEmptyImageId(Constants.MaxImageOffsets); // uint32_t
			bw.Write((uint8_t)model.CompatibleTrackObjects.Count);
			bw.WriteEmptyObjectId(Constants.MaxNumCompatible);
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);
			bw.WriteEmptyPointer(Constants.CargoOffsetBytesSize); // CargoOffsetBytes, not part of object definition
			bw.WriteEmptyPointer(Constants.var_6E_Length); // var_6E, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}

	private static void SaveVariable(TrackStationObject model, LocoBinaryWriter bw)
	{
		// compatible road objects
		bw.WriteS5HeaderList(model.CompatibleTrackObjects);

		// cargo offsets
		bw.WriteCargoOffsets(model.CargoOffsets);

		// var_6E offsets
		for (var i = 0; i < Constants.var_6E_Length; ++i)
		{
			bw.Write(model.var_6E[i]);
		}
	}

	[Flags]
	internal enum DatTrackStationObjectFlags : uint8_t
	{
		None = 0,
		Recolourable = 1 << 0,
		NoGlass = 1 << 1,
	}
}
