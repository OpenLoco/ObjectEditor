using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.ScenarioText;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class ScenarioTextObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x06;
	}

	public static ObjectType ObjectType => ObjectType.ScenarioText;
	public static DatObjectType DatObjectType => DatObjectType.ScenarioText;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new ScenarioTextObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipStringId(); // Details offset, not part of object definition
			br.SkipByte(0x06 - 0x04); // pad, not used

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			// N/A

			// image table
			// N/A but ScenaroText has an empty image table for some reason
			_ = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType, model, stringTable, null);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.WriteEmptyStringId(); // Details offset, not part of object definition
			bw.WriteEmptyBytes(0x06 - 0x04); // padding

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, new ImageTable().GraphicsElements);
		}
	}
}
