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
	[LocoStructSize(0x1F)]
	[LocoStringCount(4)]
	public record CargoObject(
		//[property: LocoStructOffset(0x00)] string_id Name,
		[property: LocoStructOffset(0x02)] uint16_t var_02,
		[property: LocoStructOffset(0x04)] uint16_t var_04,
		[property: LocoStructOffset(0x06)] string_id UnitsAndCargoName,
		[property: LocoStructOffset(0x08)] string_id UnitNameSingular,
		[property: LocoStructOffset(0x0A)] string_id UnitNamePlural,
		[property: LocoStructOffset(0x0C)] uint32_t UnitInlineSprite,
		[property: LocoStructOffset(0x10)] uint16_t MatchFlags,
		[property: LocoStructOffset(0x12)] CargoObjectFlags Flags,
		[property: LocoStructOffset(0x13)] uint8_t NumPlatformVariations,
		[property: LocoStructOffset(0x14)] uint8_t var_14,
		[property: LocoStructOffset(0x15)] uint8_t PremiumDays,
		[property: LocoStructOffset(0x16)] uint8_t MaxNonPremiumDays,
		[property: LocoStructOffset(0x17)] uint16_t MaxPremiumRate,
		[property: LocoStructOffset(0x19)] uint16_t PenaltyRate,
		[property: LocoStructOffset(0x1B)] uint16_t PaymentFactor,
		[property: LocoStructOffset(0x1D)] uint8_t PaymentIndex,
		[property: LocoStructOffset(0x1E)] uint8_t UnitSize
		) : ILocoStruct, ILocoStructStringTablePostLoad
	{
		public static ObjectType ObjectType => ObjectType.Cargo;
		public static int StructSize => 0x1F;

		public string Name { get; set; }

		public void LoadPostStringTable(StringTable stringTable)
		{
			Name = stringTable[(0, (LanguageId)0)];
		}
	}
}