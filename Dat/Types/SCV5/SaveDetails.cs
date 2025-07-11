using Dat.FileParsing;
using System.ComponentModel;

namespace Dat.Types.SCV5;

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(StructLength)]
public record SaveDetails(
	[property: LocoStructOffset(0x00), LocoArrayLength(256), Browsable(false)] char_t[] Company,
	[property: LocoStructOffset(0x100), LocoArrayLength(256), Browsable(false)] char_t[] Owner,
	[property: LocoStructOffset(0x200)] uint32_t Date,
	[property: LocoStructOffset(0x204)] uint16_t PerformanceIndex,
	[property: LocoStructOffset(0x206), LocoArrayLength(64), Browsable(false)] char_t[] Scenario,
	[property: LocoStructOffset(0x246)] uint8_t ChallengeProgress,
	[property: LocoStructOffset(0x247)] byte var_247,
	[property: LocoStructOffset(0x248), LocoArrayLength(250 * 200), Browsable(false)] uint8_t[] Image,
	[property: LocoStructOffset(0xC598)] CompanyFlags ChallengeFlags,
	[property: LocoStructOffset(0xC59C), LocoArrayLength(124), Browsable(false)] byte[] var_C59C)
	: ILocoStruct
{
	public const int StructLength = 0xC618;
	public bool Validate() => true;
}
