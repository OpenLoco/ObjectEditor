using System.ComponentModel;
using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Types;

namespace OpenLoco.ObjectEditor.Objects
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
	public record CargoObject(
		[property: LocoStructOffset(0x00), LocoString, Browsable(false)] string_id Name,
		[property: LocoStructOffset(0x02), LocoPropertyMaybeUnused] uint16_t var_02,
		[property: LocoStructOffset(0x04)] uint16_t CargoTransferTime,
		[property: LocoStructOffset(0x06), LocoString, Browsable(false)] string_id UnitsAndCargoName,
		[property: LocoStructOffset(0x08), LocoString, Browsable(false)] string_id UnitNameSingular,
		[property: LocoStructOffset(0x0A), LocoString, Browsable(false)] string_id UnitNamePlural,
		[property: LocoStructOffset(0x0C), Browsable(false)] image_id UnitInlineSprite,
		[property: LocoStructOffset(0x10)] CargoCategory CargoCategory,
		[property: LocoStructOffset(0x12)] CargoObjectFlags Flags,
		[property: LocoStructOffset(0x13)] uint8_t NumPlatformVariations,
		[property: LocoStructOffset(0x14), LocoPropertyMaybeUnused] uint8_t var_14,
		[property: LocoStructOffset(0x15)] uint8_t PremiumDays,
		[property: LocoStructOffset(0x16)] uint8_t MaxNonPremiumDays,
		[property: LocoStructOffset(0x17)] uint16_t MaxPremiumRate,
		[property: LocoStructOffset(0x19)] uint16_t PenaltyRate,
		[property: LocoStructOffset(0x1B)] uint16_t PaymentFactor,
		[property: LocoStructOffset(0x1D)] uint8_t PaymentIndex,
		[property: LocoStructOffset(0x1E)] uint8_t UnitSize
		) : ILocoStruct, ILocoImageTableNames
	{
		public bool Validate()
			=> var_02 <= 3840
			&& CargoTransferTime != 0;

		public bool TryGetImageName(int id, out string? value)
			=> ImageIdNameMap.TryGetValue(id, out value);

		public static Dictionary<int, string> ImageIdNameMap = new()
		{
			{ 0, "kInlineSprite" },
			// There are NumPlatformVariations sprites after this one
			{ 1, "kStationPlatformBegin" },
		};
	}
}
