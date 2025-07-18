using Dat.Data;
using Dat.FileParsing;
using Dat.Types;
using System.ComponentModel;

namespace Dat.Objects;

[Flags]
public enum CargoObjectFlags : uint8_t
{
	None = 0,
	unk0 = 1 << 0,
	Refit = 1 << 1,
	Delivering = 1 << 2,
}

public enum CargoCategory : uint16_t
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
[LocoStructType(ObjectType.Cargo)]
[LocoStringTable("Name", "UnitsAndCargoName", "UnitNameSingular", "UnitNamePlural")]
public record CargoObject(
	[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
	[property: LocoStructOffset(0x02)] uint16_t var_02,
	[property: LocoStructOffset(0x04)] uint16_t CargoTransferTime,
	[property: LocoStructOffset(0x06), LocoString, Browsable(false)] string_id UnitsAndCargoName,
	[property: LocoStructOffset(0x08), LocoString, Browsable(false)] string_id UnitNameSingular,
	[property: LocoStructOffset(0x0A), LocoString, Browsable(false)] string_id UnitNamePlural,
	[property: LocoStructOffset(0x0C), Browsable(false)] image_id UnitInlineSprite,
	[property: LocoStructOffset(0x10)] CargoCategory CargoCategory,
	[property: LocoStructOffset(0x12)] CargoObjectFlags Flags,
	[property: LocoStructOffset(0x13)] uint8_t NumPlatformVariations,
	[property: LocoStructOffset(0x14)] uint8_t StationCargoDensity,
	[property: LocoStructOffset(0x15)] uint8_t PremiumDays,
	[property: LocoStructOffset(0x16)] uint8_t MaxNonPremiumDays,
	[property: LocoStructOffset(0x17)] uint16_t MaxPremiumRate,
	[property: LocoStructOffset(0x19)] uint16_t PenaltyRate,
	[property: LocoStructOffset(0x1B)] uint16_t PaymentFactor,
	[property: LocoStructOffset(0x1D)] uint8_t PaymentIndex,
	[property: LocoStructOffset(0x1E)] uint8_t UnitSize
	) : ILocoStruct, IImageTableNameProvider
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
