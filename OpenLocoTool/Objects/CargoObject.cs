using System.ComponentModel;
using OpenLocoTool.DatFileParsing;
using OpenLocoTool.Headers;

namespace OpenLocoTool.Objects
{
	public enum CargoObjectFlags : uint8_t
	{
		None = 0,
		unk0 = 1 << 0,
		Refit = 1 << 1,
		Delivering = 1 << 2,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	public record CargoObject(
		[property: LocoStructProperty] string_id Name,
		[property: LocoStructProperty] uint16_t var_2,
		[property: LocoStructProperty] uint16_t var_4,
		[property: LocoStructProperty] string_id UnitsAndCargoName,
		[property: LocoStructProperty] string_id UnitNameSingular,
		[property: LocoStructProperty] string_id UnitNamePlural,
		[property: LocoStructProperty] uint32_t UnitInlineSprite,
		[property: LocoStructProperty] uint16_t MatchFlags,
		[property: LocoStructProperty] CargoObjectFlags Flags,
		[property: LocoStructProperty] uint8_t NumPlatformVariations,
		[property: LocoStructProperty] uint8_t var_14,
		[property: LocoStructProperty] uint8_t PremiumDays,
		[property: LocoStructProperty] uint8_t MaxNonPremiumDays,
		[property: LocoStructProperty] uint16_t MaxPremiumRate,
		[property: LocoStructProperty] uint16_t PenaltyRate,
		[property: LocoStructProperty] uint16_t PaymentFactor,
		[property: LocoStructProperty] uint8_t PaymentIndex,
		[property: LocoStructProperty] uint8_t UnitSize) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.cargo;
		public int ObjectStructSize => 0x1F;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
		{
			var name = "todo: implement code to lookup string table";

			var byteReader = new ByteReader(data);
			return new CargoObject(
				0,
				byteReader.Read<uint16_t>(),
				byteReader.Read<uint16_t>(),
				byteReader.Read<string_id>(),
				byteReader.Read<string_id>(),
				byteReader.Read<string_id>(),
				byteReader.Read<uint32_t>(),
				byteReader.Read<uint16_t>(),
				byteReader.Read<CargoObjectFlags>(),
				byteReader.Read<uint8_t>(),
				byteReader.Read<uint8_t>(),
				byteReader.Read<uint8_t>(),
				byteReader.Read<uint8_t>(),
				byteReader.Read<uint16_t>(),
				byteReader.Read<uint16_t>(),
				byteReader.Read<uint16_t>(),
				byteReader.Read<uint8_t>(),
				byteReader.Read<uint8_t>()
				);
		}

		public ReadOnlySpan<byte> Write() => throw new NotImplementedException();
	}
}