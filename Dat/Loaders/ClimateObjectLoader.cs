using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Climate;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class ClimateObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int Seasons = 4;
	}

	public static ObjectType ObjectType => ObjectType.Climate;
	public static DatObjectType DatObjectType => DatObjectType.Climate;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new ClimateObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.FirstSeason = (Season)br.ReadByte();
			model.SeasonLength1 = br.ReadByte();
			model.SeasonLength2 = br.ReadByte();
			model.SeasonLength3 = br.ReadByte();
			model.SeasonLength4 = br.ReadByte();
			model.WinterSnowLine = br.ReadByte();
			model.SummerSnowLine = br.ReadByte();
			model.pad_09 = br.ReadByte();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + ObjectAttributes.StructSize(DatObjectType), nameof(stream.Position));

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			// N/A

			// image table
			// N/A but Climate has an empty image table for some reason
			_ = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType, model, stringTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (ClimateObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write((uint8_t)model.FirstSeason);
			bw.Write(model.SeasonLength1);
			bw.Write(model.SeasonLength2);
			bw.Write(model.SeasonLength3);
			bw.Write(model.SeasonLength4);
			bw.Write(model.WinterSnowLine);
			bw.Write(model.SummerSnowLine);
			bw.Write(model.pad_09);

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
