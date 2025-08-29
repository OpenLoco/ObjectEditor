using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Climate;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class ClimateObjectLoader : IDatObjectLoader
{
	public static class Constants
	{
		public const int Seasons = 4;
	}

	public static class StructSizes
	{
		public const int Dat = 0x0A;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new ClimateObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.FirstSeason = br.ReadByte();
			model.SeasonLength1 = br.ReadByte();
			model.SeasonLength2 = br.ReadByte();
			model.SeasonLength3 = br.ReadByte();
			model.SeasonLength4 = br.ReadByte();
			model.WinterSnowLine = br.ReadByte();
			model.SummerSnowLine = br.ReadByte();
			_ = br.ReadByte(); // pad

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Climate), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.Climate, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (ClimateObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.FirstSeason);
			bw.Write(model.SeasonLength1);
			bw.Write(model.SeasonLength2);
			bw.Write(model.SeasonLength3);
			bw.Write(model.SeasonLength4);
			bw.Write(model.WinterSnowLine);
			bw.Write(model.SummerSnowLine);
			bw.Write((uint8_t)0); // pad

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

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x0A)]
[LocoStructType(DatObjectType.Climate)]
internal record DatClimateObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint8_t FirstSeason,
	[property: LocoStructOffset(0x03), LocoArrayLength(ClimateObjectLoader.Constants.Seasons)] uint8_t[] SeasonLengths,
	[property: LocoStructOffset(0x07)] uint8_t WinterSnowLine,
	[property: LocoStructOffset(0x08)] uint8_t SummerSnowLine,
	[property: LocoStructOffset(0x09), LocoPropertyMaybeUnused] uint8_t pad_09
	)
{

	public bool Validate()
		=> WinterSnowLine <= SummerSnowLine
		&& FirstSeason < 4;
}
