using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Types;
using System.ComponentModel;

namespace Dat.Loaders;

public abstract class CargoObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x1F;
	}

	public static LocoObject Load(MemoryStream stream)
	{
		var initialStreamPosition = stream.Position;

		using (var br = new LocoBinaryReader(stream))
		{
			var model = new CargoObject();
			var stringTable = new StringTable();
			var imageTable = new List<GraphicsElement>();

			// fixed
			br.SkipStringId(); // Name offset, not part of object definition
			model.var_02 = br.ReadUInt16(); // var_02, not used in Locomotion
			model.CargoTransferTime = br.ReadUInt16();
			br.SkipStringId(); // UnitsAndCargoName, not part of object definition
			br.SkipStringId(); // UnitNameSingular, not part of object definition
			br.SkipStringId(); // UnitNamePlural, not part of object definition
			br.SkipImageId(); // UnitInlineSprite offset, not part of object definition
			model.CargoCategory = (CargoCategory)br.ReadUInt16();
			model.Flags = (CargoObjectFlags)br.ReadByte();
			model.NumPlatformVariations = br.ReadByte();
			model.StationCargoDensity = br.ReadByte();
			model.PremiumDays = br.ReadByte();
			model.MaxNonPremiumDays = br.ReadByte();
			model.MaxPremiumRate = br.ReadUInt16();
			model.PenaltyRate = br.ReadUInt16();
			model.PaymentFactor = br.ReadUInt16();
			model.PaymentIndex = br.ReadByte();
			model.UnitSize = br.ReadByte();

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			stringTable = SawyerStreamReader.ReadStringTableStream(stream, ObjectAttributes.StringTable(DatObjectType.Cargo), null);

			// variable
			// N/A

			// image table
			imageTable = SawyerStreamReader.ReadImageTableStream(stream).Table;

			return new LocoObject(ObjectType.Cargo, model, stringTable, imageTable);
		}
	}

	public static void Save(MemoryStream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (CargoObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteStringId(); // Name offset, not part of object definition
			bw.Write(model.var_02);
			bw.Write(model.CargoTransferTime);
			bw.WriteStringId(); // UnitsAndCargoName, not part of object definition
			bw.WriteStringId(); // UnitNameSingular, not part of object definition
			bw.WriteStringId(); // UnitNamePlural, not part of object definition
			bw.WriteImageId(); // UnitInlineSprite offset, not part of object definition
			bw.Write((uint16_t)model.CargoCategory);
			bw.Write((uint8_t)model.Flags);
			bw.Write(model.NumPlatformVariations);
			bw.Write(model.StationCargoDensity);
			bw.Write(model.PremiumDays);
			bw.Write(model.MaxNonPremiumDays);
			bw.Write(model.MaxPremiumRate);
			bw.Write(model.PenaltyRate);
			bw.Write(model.PaymentFactor);
			bw.Write(model.PaymentIndex);
			bw.Write(model.UnitSize);

			// sanity check
			ArgumentOutOfRangeException.ThrowIfNotEqual(stream.Position, initialStreamPosition + StructSizes.Dat, nameof(stream.Position));

			// string table
			SawyerStreamWriter.WriteStringTableStream(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTableStream(stream, obj.GraphicsElements);
		}
	}
}

[Flags]
internal enum DatCargoObjectFlags : uint8_t
{
	None = 0,
	unk0 = 1 << 0,
	Refit = 1 << 1,
	Delivering = 1 << 2,
}

internal enum DatCargoCategory : uint16_t
{
	None = 0,
	Grain = 1,
	Coal = 2,
	IronOre = 3,
	Liquids = 4,
	Paper = 5,
	Steel = 6,
	Timber = 7,
	Goods = 8,
	Foods = 9,
	//<unused> = 10
	Livestock = 11,
	Passengers = 12,
	Mail = 13,
	// Note: Mods may (and do) use other numbers not covered by this list
	NULL = (uint16_t)0xFFFFU,
}

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x1F)]
[LocoStructType(DatObjectType.Cargo)]
internal record DatCargoObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint16_t var_02,
	[property: LocoStructOffset(0x04)] uint16_t CargoTransferTime,
	[property: LocoStructOffset(0x06), LocoString, Browsable(false)] string_id UnitsAndCargoName,
	[property: LocoStructOffset(0x08), LocoString, Browsable(false)] string_id UnitNameSingular,
	[property: LocoStructOffset(0x0A), LocoString, Browsable(false)] string_id UnitNamePlural,
	[property: LocoStructOffset(0x0C), Browsable(false)] image_id UnitInlineSprite,
	[property: LocoStructOffset(0x10)] DatCargoCategory CargoCategory,
	[property: LocoStructOffset(0x12)] DatCargoObjectFlags Flags,
	[property: LocoStructOffset(0x13)] uint8_t NumPlatformVariations,
	[property: LocoStructOffset(0x14)] uint8_t StationCargoDensity,
	[property: LocoStructOffset(0x15)] uint8_t PremiumDays,
	[property: LocoStructOffset(0x16)] uint8_t MaxNonPremiumDays,
	[property: LocoStructOffset(0x17)] uint16_t MaxPremiumRate,
	[property: LocoStructOffset(0x19)] uint16_t PenaltyRate,
	[property: LocoStructOffset(0x1B)] uint16_t PaymentFactor,
	[property: LocoStructOffset(0x1D)] uint8_t PaymentIndex,
	[property: LocoStructOffset(0x1E)] uint8_t UnitSize
	) : IImageTableNameProvider
{
	public bool Validate()
		=> var_02 <= 3840
		&& CargoTransferTime != 0;

	public bool TryGetImageName(int id, out string? value)
	{
		value = id == 0
			? "kInlineSprite"
			: $"kStationPlatform{id}";

		return true;
	}
}
