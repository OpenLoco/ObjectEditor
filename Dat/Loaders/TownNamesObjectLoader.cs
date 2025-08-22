using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Streetlight;
using Definitions.ObjectModels.Objects.TownNames;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

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
		public const int Category = 0x04; // 4 bytes
	}

	public static LocoObject Load(MemoryStream stream)
	{
		using (var br = new LocoBinaryReader(stream))
		{
			var model = new TownNamesObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			_ = br.SkipStringId();
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
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.TownNames), null);

			// variable
			// N/A

			// image table
			// N/A

			return new LocoObject(ObjectType.TownNames, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var model = obj.Object as TownNamesObject;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId();// Name offset, not part of object definition
			for (var i = 0; i < Constants.Categories; ++i)
			{
				bw.WriteByte(model.Categories[i].Count);
				bw.WriteByte(model.Categories[i].Bias);
				bw.WriteUInt16(model.Categories[i].Offset);
			}

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			// N/A
		}
	}

}

[LocoStructSize(0x1A)]
[LocoStructType(DatObjectType.TownNames)]
internal record DatTownNamesObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), LocoArrayLength(6)] Category[] Categories
) : ILocoStruct, ILocoStructVariableData
{
	byte[] tempUnkVariableData;

	public ReadOnlySpan<byte> LoadVariable(ReadOnlySpan<byte> remainingData)
	{
		// town names is interesting - loco has not RE'd the whole object and there are no graphics, so we just
		// skip the rest of the data/object
		tempUnkVariableData = remainingData.ToArray();
		return remainingData[remainingData.Length..];
	}

	public ReadOnlySpan<byte> SaveVariable()
		=> tempUnkVariableData;

	public bool Validate() => true;
}
