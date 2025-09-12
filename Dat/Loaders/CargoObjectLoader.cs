using Dat.Data;
using Dat.FileParsing;
using Definitions.ObjectModels;
using Definitions.ObjectModels.Objects.Cargo;
using Definitions.ObjectModels.Types;

namespace Dat.Loaders;

public abstract class CargoObjectLoader : IDatObjectLoader
{
	public static class Constants
	{ }

	public static class StructSizes
	{
		public const int Dat = 0x1F;
	}

	public static LocoObject Load(Stream stream)
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
			imageTable = SawyerStreamReader.ReadImageTable(br).Table;

			return new LocoObject(ObjectType.Cargo, model, stringTable, imageTable);
		}
	}

	public static void Save(Stream stream, LocoObject obj)
	{
		var initialStreamPosition = stream.Position;
		var model = (CargoObject)obj.Object;

		using (var bw = new LocoBinaryWriter(stream))
		{
			bw.WriteEmptyStringId(); // Name offset, not part of object definition
			bw.Write(model.var_02);
			bw.Write(model.CargoTransferTime);
			bw.WriteEmptyStringId(); // UnitsAndCargoName, not part of object definition
			bw.WriteEmptyStringId(); // UnitNameSingular, not part of object definition
			bw.WriteEmptyStringId(); // UnitNamePlural, not part of object definition
			bw.WriteEmptyImageId(); // UnitInlineSprite offset, not part of object definition
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
			SawyerStreamWriter.WriteStringTable(stream, obj.StringTable);

			// variable
			// N/A

			// image table
			SawyerStreamWriter.WriteImageTable(stream, obj.ImageTable.GraphicsElements);
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
