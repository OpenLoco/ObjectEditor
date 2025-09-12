using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Scaffolding;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class ScaffoldingObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int SegmentHeightCount = 3;
		public const int RoofHeightCount = 3;
	}

	public static class StructSizes
	{
		public const int Dat = 0x12;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new ScaffoldingObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipImageId(); // Image, not part of object definition

			for (var i = 0; i < Constants.SegmentHeightCount; i++)
			{
				model.SegmentHeights.Add(br.ReadUInt16());
			}

			for (var i = 0; i < Constants.RoofHeightCount; i++)
			{
				model.RoofHeights.Add(br.ReadUInt16());
			}

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Scaffolding), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.Scaffolding, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = obj.Object as ScaffoldingObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.WriteEmptyImageId();

			for (var i = 0; i < Constants.SegmentHeightCount; i++)
			{
				bw.Write(model.SegmentHeights[i]);
			}

			for (var i = 0; i < Constants.RoofHeightCount; i++)
			{
				bw.Write(model.RoofHeights[i]);
			}

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}
}
