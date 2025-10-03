using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Graphics;
using Definitions.ObjectModels.Objects.Streetlight;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class StreetLightObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int DesignedYearLength = 3;
	}

	public static class StructSizes
	{
		public const int Dat = 0x0C;
	}

	public static ObjectType ObjectType => ObjectType.StreetLight;
	public static DatObjectType DatObjectType => DatObjectType.StreetLight;

	public static LocoObject Load(Stream stream)
	{
		using (var br = new LocoBinaryReader(stream))
		{
			var model = new StreetLightObject();

			// fixed
			br.SkipStringId();
			for (var i = 0; i < Constants.DesignedYearLength; ++i)
			{
				model.DesignedYears.Add(br.ReadUInt16());
			}

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
		var model = obj.Object as StreetLightObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId();// Name offset, not part of object definition
			for (var i = 0; i < Constants.DesignedYearLength; ++i)
			{
				bw.Write(model.DesignedYears[i]);
			}

			// string table
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
		}
	}
}

[LocoStructSize(0x0C)]
[LocoStructType(DatObjectType.StreetLight)]
public record DatStreetLightObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), LocoArrayLength(StreetLightObjectLoader.Constants.DesignedYearLength)] uint16_t[] DesignedYears
)
{ }
