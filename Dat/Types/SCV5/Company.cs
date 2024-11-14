using OpenLoco.Dat.FileParsing;

namespace OpenLoco.Dat.Types.SCV5
{
	[LocoStructSize(0x8FA8)]
	public class Company
	{
		public uint16_t Name { get; set; }                 // 0x0000
		public uint16_t OwnerName { get; set; }            // 0x0002
		public CompanyFlags ChallengeFlags { get; set; }   // 0x0004
		[LocoArrayLength(6)] uint8_t[] Cash { get; set; }               // 0x0008
		public uint32_t CurrentLoan { get; set; }          // 0x000E
		public uint32_t UpdateCounter { get; set; }        // 0x0012
		public int16_t PerformanceIndex { get; set; }      // 0x0016
		[LocoArrayLength(0x8C4E - 0x18)] public uint8_t[] var_18 { get; set; } // 0x0018
		public uint8_t ChallengeProgress { get; set; }     // 0x8C4E
		[LocoArrayLength(0x8FA8 - 0x8C4F)] public uint8_t[] var_8C4F { get; set; }
	}
}
