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

	public enum CargoCategory : uint16_t
	{
		grain = 1,
		coal,
		ironOre,
		liquids,
		paper,
		steel,
		timber,
		goods,
		foods,
		livestock = 11,
		passengers,
		mail,
		// Note: Mods may (and do) use other numbers not covered by this list
		NULL = (uint16_t)0xFFFFU,
	};

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0x1F)]
	[LocoStructType(ObjectType.Cargo)]
	[LocoStringTable("Name", "UnitsAndCargoName", "UnitNameSingular", "UnitNamePlural")]
	public class CargoObject(
		uint16_t var_02,
		uint16_t cargoTransferTime,
		CargoCategory cargoCategory,
		CargoObjectFlags flags,
		uint8_t numPlatformVariations,
		uint8_t var_14,
		uint8_t premiumDays,
		uint8_t maxNonPremiumDays,
		uint16_t maxPremiumRate,
		uint16_t penaltyRate,
		uint16_t paymentFactor,
		uint8_t paymentIndex,
		uint8_t unitSize)
		: ILocoStruct
	{
		//[LocoStructOffset(0x00), LocoString, Browsable(false)] public string_id Name { get; set; }
		[LocoStructOffset(0x02), LocoPropertyMaybeUnused] public uint16_t var_02 { get; set; } = var_02;
		[LocoStructOffset(0x04)] public uint16_t CargoTransferTime { get; set; } = cargoTransferTime;
		//[LocoStructOffset(0x06), LocoString, Browsable(false)] public string_id UnitsAndCargoName { get; set; }
		//[LocoStructOffset(0x08), LocoString, Browsable(false)] public string_id UnitNameSingular { get; set; }
		//[LocoStructOffset(0x0A), LocoString, Browsable(false)] public string_id UnitNamePlural { get; set; }
		//[LocoStructOffset(0x0C)] public image_id UnitInlineSprite { get; set; }
		[LocoStructOffset(0x10)] public CargoCategory CargoCategory { get; set; } = cargoCategory;
		[LocoStructOffset(0x12)] public CargoObjectFlags Flags { get; set; } = flags;
		[LocoStructOffset(0x13)] public uint8_t NumPlatformVariations { get; set; } = numPlatformVariations;
		[LocoStructOffset(0x14), LocoPropertyMaybeUnused] public uint8_t var_14 { get; set; } = var_14;
		[LocoStructOffset(0x15)] public uint8_t PremiumDays { get; set; } = premiumDays;
		[LocoStructOffset(0x16)] public uint8_t MaxNonPremiumDays { get; set; } = maxNonPremiumDays;
		[LocoStructOffset(0x17)] public uint16_t MaxPremiumRate { get; set; } = maxPremiumRate;
		[LocoStructOffset(0x19)] public uint16_t PenaltyRate { get; set; } = penaltyRate;
		[LocoStructOffset(0x1B)] public uint16_t PaymentFactor { get; set; } = paymentFactor;
		[LocoStructOffset(0x1D)] public uint8_t PaymentIndex { get; set; } = paymentIndex;
		[LocoStructOffset(0x1E)] public uint8_t UnitSize { get; set; } = unitSize;
	}
}