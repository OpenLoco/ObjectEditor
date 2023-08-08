using System.ComponentModel;
using System.Runtime.CompilerServices;
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
		string Name,
		uint16_t var_2,
		uint16_t var_4,
		string_id UnitsAndCargoName,
		string_id UnitNameSingular,
		string_id UnitNamePlural,
		uint32_t UnitInlineSprite,
		uint16_t MatchFlags,
		CargoObjectFlags Flags,
		uint8_t NumPlatformVariations,
		uint8_t var_14,
		uint8_t PremiumDays,
		uint8_t MaxNonPremiumDays,
		uint16_t MaxPremiumRate,
		uint16_t PenaltyRate,
		uint16_t PaymentFactor,
		uint8_t PaymentIndex,
		uint8_t UnitSize) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.cargo;
		public int ObjectStructSize => 0x1F;

		public static ILocoStruct Read(ReadOnlySpan<byte> data)
		{
			var name = "todo: implement code to lookup string table";

			var byteReader = new ByteReader(data);
			return new CargoObject(
				name,
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