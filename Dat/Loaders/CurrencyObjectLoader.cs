using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Currency;
using Definitions.ObjectModels.Types;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Dat.Loaders;

public abstract class CurrencyObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x0C;
	}

	public static ObjectType ObjectType => ObjectType.Currency;
	public static DatObjectType DatObjectType => DatObjectType.Currency;

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new CurrencyObject();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipStringId(); // PrefixSymbol, not part of object definition
			br.SkipStringId(); // SuffixSymbol, not part of object definition
			br.SkipImageId(); // ObjectIcon, not part of object definition
			model.Separator = br.ReadByte();
			model.Factor = br.ReadByte();

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
		var model = (CurrencyObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.WriteEmptyStringId(); // PrefixSymbol, not part of object definition
			bw.WriteEmptyStringId(); // SuffixSymbol, not part of object definition
			bw.WriteEmptyImageId(); // ObjectIcon, not part of object definition
			bw.Write(model.Separator);
			bw.Write(model.Factor);

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

	public bool TryGetImageName(int id, [MaybeNullWhen(false)] out string value) => throw new NotImplementedException();
}

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x0C)]
[LocoStructType(DatObjectType.Currency)]
internal record DatCurrencyObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02), LocoString, Browsable(false)] string_id PrefixSymbol,
	[property: LocoStructOffset(0x04), LocoString, Browsable(false)] string_id SuffixSymbol,
	[property: LocoStructOffset(0x06), Browsable(false)] image_id ObjectIcon,
	[property: LocoStructOffset(0x0A)] uint8_t Separator,
	[property: LocoStructOffset(0x0B)] uint8_t Factor
	)
{
	public bool Validate()
	{
		if (Separator > 4)
		{
			return false;
		}

		if (Factor > 3)
		{
			return false;
		}

		return true;
	}
}
