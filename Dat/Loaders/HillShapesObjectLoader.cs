using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.HillShape;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class HillShapesObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x0E;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new HillShapesObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.HillHeightMapCount = br.ReadByte();
			model.MountainHeightMapCount = br.ReadByte();
			br.SkipImageId(); // Image, not part of object definition
			br.SkipImageId(); // ImageHill, not part of object definition
			model.IsHeightMap = ((DatHillShapeFlags)br.ReadUInt16()).HasFlag(DatHillShapeFlags.IsHeightMap);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.HillShapes), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.HillShapes, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (HillShapesObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition
			bw.Write(model.HillHeightMapCount);
			bw.Write(model.MountainHeightMapCount);
			bw.WriteImageId();
			bw.WriteImageId();
			bw.Write((uint16_t)(model.IsHeightMap ? DatHillShapeFlags.IsHeightMap : DatHillShapeFlags.None));

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}

	[Flags]
	internal enum DatHillShapeFlags : uint16_t
	{
		None = 0,
		IsHeightMap = 1 << 0,
	}
}
