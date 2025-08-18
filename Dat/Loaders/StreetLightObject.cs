using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Streetlight;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Objects;

public abstract class StreetLightObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int DesignedYearLength = 3;
	}

	public static class Sizes
	{ }

	public static LocoObject Load(MemoryStream stream)
	{
		using (var br = new LocoBinaryReader(stream))
		{
			var model = new StreetLightObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId();
			for (var i = 0; i < Constants.DesignedYearLength; ++i)
			{
				model.DesignedYears.Add(br.ReadUInt16());
			}

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.StreetLight), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.StreetLight, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var model = obj.Object as StreetLightObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId();// Name offset, not part of object definition
			for (var i = 0; i < Constants.DesignedYearLength; ++i)
			{
				bw.WriteUint16(model.DesignedYears[i]);
			}

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
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
