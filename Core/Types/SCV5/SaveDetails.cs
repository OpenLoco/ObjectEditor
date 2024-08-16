using OpenLoco.Dat.FileParsing;
using OpenLoco.Dat.Types;

namespace OpenLoco.Dat.Types.SCV5
{
	[LocoStructSize(StructLength)]
	public record SaveDetails([property: LocoStructOffset(0x00), LocoArrayLength(256)] char_t[] Company,
		[property: LocoStructOffset(0x100), LocoArrayLength(256)] char_t[] Owner,
		[property: LocoStructOffset(0x200)] uint32_t Date,
		[property: LocoStructOffset(0x204)] uint16_t PerformanceIndex,
		[property: LocoStructOffset(0x206), LocoArrayLength(64)] char_t[] Scenario,
		[property: LocoStructOffset(0x246)] uint8_t ChallengeProgress,
		[property: LocoStructOffset(0x247)] byte pad_247,
		[property: LocoStructOffset(0x248), LocoArrayLength(250 * 200)] uint8_t[] Image,
		[property: LocoStructOffset(0xC598)] CompanyFlags ChallengeFlags,
		[property: LocoStructOffset(0xC59C), LocoArrayLength(124)] byte[] pad_C59C)
		: ILocoStruct
	{
		public const int StructLength = 0xC618;
		public bool Validate() => true;
	}
}
