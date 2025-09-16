using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Water;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;
public abstract class WaterObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x0E;
	}

	public static ObjectType ObjectType => ObjectType.Water;
	public static DatObjectType DatObjectType => DatObjectType.Water;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new WaterObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.CostIndex = br.ReadByte();
			model.var_03 = br.ReadByte();
			model.CostFactor = br.ReadInt16();
			br.SkipImageId(); // ImageId, not part of object definition
			br.SkipImageId(); // MapPixelImage, not part of object definition

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
		var model = (WaterObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.CostIndex);
			bw.Write(model.var_03);
			bw.Write(model.CostFactor);
			bw.WriteEmptyImageId(); // ImageId, not part of object definition
			bw.WriteEmptyImageId(); // MapPixelImage, not part of object definition

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
