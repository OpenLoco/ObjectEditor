using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.TownNames;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class TownNamesObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int MinNumNameCombinations = 80;
		public const int Categories = 6;
	}

	public static class StructSizes
	{
		public const int Category = 0x1A;
	}

	public static ObjectType ObjectType => ObjectType.TownNames;
	public static DatObjectType DatObjectType => DatObjectType.TownNames;

	public static LocoObject Load(Stream stream)
	{
		using (var br = new LocoBinaryReader(stream))
		{
			var model = new TownNamesObject();

			// fixed
			br.SkipStringId();
			for (var i = 0; i < Constants.Categories; ++i)
			{
				Category category = new()
				{
					Count = br.ReadByte(),
					Bias = br.ReadByte(),
					Offset = br.ReadUInt16()
				};

				model.Categories.Add(category);
			}

			// string table
			var stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType), null);

			// variable
			// N/A

			// image table
			// N/A

			return new LocoObject(ObjectType, model, stringTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var model = obj.Object as TownNamesObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId();// Name offset, not part of object definition
			for (var i = 0; i < Constants.Categories; ++i)
			{
				bw.Write(model.Categories[i].Count);
				bw.Write(model.Categories[i].Bias);
				bw.Write(model.Categories[i].Offset);
			}

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			// N/A
		}
	}

}
