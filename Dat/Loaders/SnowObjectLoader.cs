using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Snow;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class SnowObjectLoader : IDatObjectLoader
{
	public static class StructSizes
	{
		public const int DatStructSize = 0x06;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		using (var br = new LocoBinaryReader(stream))
		{
			var model = new SnowObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId(); // Name offset, not part of object definition
			_ = br.SkipImageId(); // Image offset, not part of object definition

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, StructSizes.DatStructSize, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Snow), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Snow, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition
			bw.WriteImageId(); // Image offset, not part of object definition

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}
}
