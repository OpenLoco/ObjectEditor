using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Currency;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class CurrencyObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x0C;
	}

	public static LocoObject Load(Stream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new CurrencyObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			br.SkipStringId(); // PrefixSymbol, not part of object definition
			br.SkipStringId(); // SuffixSymbol, not part of object definition
			br.SkipImageId(); // ObjectIcon, not part of object definition
			model.Separator = br.ReadByte();
			model.Factor = br.ReadByte();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Currency), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.Currency, model, stringTable, imageTable);
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
