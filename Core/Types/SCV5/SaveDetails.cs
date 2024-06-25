using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	class SaveDetails
	{
		[LocoArrayLength(256)]
		char[] Company { get; set; }                   // 0x000
		[LocoArrayLength(256)]
		char[] Owner { get; set; }                     // 0x100
		uint32_t Date;                       // 0x200
		uint16_t PerformanceIndex;           // 0x204 (from [company.performance_index)
		[LocoArrayLength(0x40)]
		char[] Scenario { get; set; }                 // 0x206
		uint8_t ChallengeProgress;           // 0x246
		byte pad_247;                   // 0x247
		[LocoArrayLength(250 * 200)]
		uint8_t[] Image { get; set; }            // 0x248
		CompanyFlags ChallengeFlags;         // 0xC598 (from [company.challenge_flags])
		[LocoArrayLength(0xC618 - 0xC59C)] // 0x7C, 124
		byte[] pad_C59C { get; set; } // 0xC59C
	}
}
