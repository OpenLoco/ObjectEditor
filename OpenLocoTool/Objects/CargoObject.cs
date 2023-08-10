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
		[property: LocoStructProperty(0x00)] string_id Name,
		[property: LocoStructProperty(0x02)] uint16_t var_2,
		[property: LocoStructProperty(0x04)] uint16_t var_4,
		[property: LocoStructProperty(0x06)] string_id UnitsAndCargoName,
		[property: LocoStructProperty(0x08)] string_id UnitNameSingular,
		[property: LocoStructProperty(0x0A)] string_id UnitNamePlural,
		[property: LocoStructProperty(0x0C)] uint32_t UnitInlineSprite,
		[property: LocoStructProperty(0x10)] uint16_t MatchFlags,
		[property: LocoStructProperty(0x12)] CargoObjectFlags Flags,
		[property: LocoStructProperty(0x13)] uint8_t NumPlatformVariations,
		[property: LocoStructProperty(0x14)] uint8_t var_14,
		[property: LocoStructProperty(0x15)] uint8_t PremiumDays,
		[property: LocoStructProperty(0x16)] uint8_t MaxNonPremiumDays,
		[property: LocoStructProperty(0x17)] uint16_t MaxPremiumRate,
		[property: LocoStructProperty(0x19)] uint16_t PenaltyRate,
		[property: LocoStructProperty(0x1B)] uint16_t PaymentFactor,
		[property: LocoStructProperty(0x1D)] uint8_t PaymentIndex,
		[property: LocoStructProperty(0x1E)] uint8_t UnitSize
		) : ILocoStruct
	{
		public ObjectType ObjectType => ObjectType.cargo;
		public static int StructLength => 0x1F;
	}
}