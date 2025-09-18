using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.TrackExtra;
using Definitions.ObjectModels.Types;
using static Dat.Loaders.TrackObjectLoader;

namespace Dat.Loaders;

public abstract class TrackExtraObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x12;
	}

	public static ObjectType ObjectType => ObjectType.TrackExtra;
	public static DatObjectType DatObjectType => DatObjectType.TrackExtra;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new TrackExtraObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.TrackPieces = ((DatTrackTraitFlags)br.ReadUInt16()).Convert();
			model.PaintStyle = br.ReadByte();
			model.CostIndex = br.ReadByte();
			model.BuildCostFactor = br.ReadInt16();
			model.SellCostFactor = br.ReadInt16();
			br.SkipImageId(); // Image offset, not part of object definition
			br.SkipImageId(); // BaseImageOffset, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			// N/A

			// image table
			var imageList = SawyerStreamReader.ReadImageTable(br).Table;

			// define groups
			var imageTable = ImageTableGrouper.CreateImageTable(model, ObjectType, imageList);

			return new LocoObject(ObjectType, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (TrackExtraObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write((uint16_t)model.TrackPieces);
			bw.Write(model.PaintStyle);
			bw.Write(model.CostIndex);
			bw.Write(model.BuildCostFactor);
			bw.Write(model.SellCostFactor);
			bw.WriteEmptyImageId(); // Image offset, not part of object definition
			bw.WriteEmptyImageId(); // BaseImageOffset, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}
}
