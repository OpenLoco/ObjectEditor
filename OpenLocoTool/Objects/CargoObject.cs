using System.Runtime.InteropServices;

namespace OpenLocoTool.Objects
{
	enum CargoObjectFlags : uint8_t
	{
		none = 0,
		unk0 = 1 << 0,
		refit = 1 << 1,
		delivering = 1 << 2,
	};

	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 0x1F)]
	struct CargoObject
	{
		// static constexpr auto kObjectType = ObjectType::cargo;

		public string_id name { get; set; } // 0x0
		public uint16_t var_2 { get; set; }
		public uint16_t var_4 { get; set; }
		public string_id unitsAndCargoName { get; set; }   // 0x6
		public string_id unitNameSingular { get; set; }    // 0x8
		public string_id unitNamePlural { get; set; }      // 0xA
		public uint32_t unitInlineSprite { get; set; }     // 0xC
		public uint16_t matchFlags { get; set; }           // 0x10
		public CargoObjectFlags flags { get; set; }        // 0x12
		public uint8_t numPlatformVariations { get; set; } // 0x13
		public uint8_t var_14 { get; set; }
		public uint8_t premiumDays { get; set; }       // 0x15
		public uint8_t maxNonPremiumDays { get; set; } // 0x16
		public uint16_t nonPremiumRate { get; set; }   // 0x17
		public uint16_t penaltyRate { get; set; }      // 0x19
		public uint16_t paymentFactor { get; set; }    // 0x1B
		public uint8_t paymentIndex { get; set; }      // 0x1D
		public uint8_t unitSize { get; set; }          // 0x1E
	}
}