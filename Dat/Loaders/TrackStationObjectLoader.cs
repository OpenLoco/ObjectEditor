using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Track;
using Definitions.ObjectModels.Objects.TrackStation;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

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

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new TrackStationObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId(); // Name offset, not part of object definition
			model.PaintStyle = br.ReadByte();
			model.Height = br.ReadByte();
			model.TrackPieces = (TrackTraitFlags)br.ReadUInt16();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			model.CostIndex = br.ReadByte();
			model.var_0B = br.ReadByte();
			model.Flags = (TrackStationObjectFlags)br.ReadByte();
			model.var_0D = br.ReadByte();
			_ = br.SkipImageId(); // Image, not part of object definition
			_ = br.SkipImageId(Constants.MaxImageOffsets);
			var compatibleTrackObjectCount = br.ReadByte();
			_ = br.SkipObjectId(Constants.MaxNumCompatible);
			model.DesignedYear = br.ReadUInt16();
			model.ObsoleteYear = br.ReadUInt16();
			_ = br.SkipPointer(Constants.CargoOffsetBytesSize); // CargoOffsetBytes, not part of object definition
			_ = br.SkipPointer(Constants.var_6E_Length); // CargoOffsetBytes, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.TrackStation), null);

			// variable
			LoadVariable(br, model, compatibleTrackObjectCount);

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.TrackStation, model, stringTable, imageTable);
		}
	}

	private static void LoadVariable(LocoBinaryReader br, TrackStationObject model, byte compatibleRoadObjectCount)
	{
		// compatible road objects
		model.CompatibleTrackObjects = br.ReadS5HeaderList(compatibleRoadObjectCount);

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

		// very similar to cargo offset bytes
		model.var_6E = new byte[Constants.var_6E_Length][];
		for (var i = 0; i < Constants.var_6E_Length; ++i)
		{
			var length = 1;
			while (br.PeekByte(length) != 0xFF)
			{
				length += 4; // x, y, x, y
			}

			length += 4;
			model.var_6E[i] = br.ReadBytes(length);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (TrackStationObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition
			bw.Write(model.PaintStyle);
			bw.Write(model.Height);
			bw.Write((uint16_t)model.TrackPieces);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.Write(model.CostIndex);
			bw.Write(model.var_0B);
			bw.Write((uint8_t)model.Flags);
			bw.Write(model.var_0D);
			bw.WriteImageId(); // Image, not part of object definition
			bw.WriteImageId(Constants.MaxImageOffsets); // uint32_t
			bw.Write((uint8_t)model.CompatibleTrackObjects.Count);
			bw.WriteObjectId(Constants.MaxNumCompatible);
			bw.Write(model.DesignedYear);
			bw.Write(model.ObsoleteYear);
			bw.WritePointer(Constants.CargoOffsetBytesSize); // CargoOffsetBytes, not part of object definition
			bw.WritePointer(Constants.var_6E_Length); // var_6E, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			SaveVariable(model, bw);

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}

	private static void SaveVariable(TrackStationObject model, LocoBinaryWriter bw)
	{
		// compatible road objects
		bw.WriteS5HeaderList(model.CompatibleTrackObjects);

		// cargo offsets
		for (var i = 0; i < 4; ++i)
		{
			for (var j = 0; j < 4; ++j)
			{
				bw.Write(model.CargoOffsetBytes[i][j]);
			}
		}

		// var_6E offsets
		for (var i = 0; i < Constants.var_6E_Length; ++i)
		{
			bw.Write(model.var_6E[i]);
		}
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
}
