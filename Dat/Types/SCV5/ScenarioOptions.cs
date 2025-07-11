using Dat.FileParsing;
using System.ComponentModel;

namespace Dat.Types.SCV5
{
	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(StructLength)]
	public record ScenarioOptions(
		[property: LocoStructOffset(0x00)] EditorControllerStep EditorStep,
		[property: LocoStructOffset(0x01)] uint8_t Difficulty,
		[property: LocoStructOffset(0x02)] uint16_t ScenarioStartYear,
		[property: LocoStructOffset(0x04), LocoArrayLength(2)] uint8_t[] var_4,
		[property: LocoStructOffset(0x06)] ScenarioFlags ScenarioFlags,
		[property: LocoStructOffset(0x08)] uint8_t MadeAnyChanges,
		[property: LocoStructOffset(0x09)] uint8_t var_9,
		[property: LocoStructOffset(0x0A), LocoArrayLength(32)] LandDistributionPattern LandDistributionPatterns,
		[property: LocoStructOffset(0x2A), LocoArrayLength(64), Browsable(false)] char_t[] ScenarioName, // this is a string
		[property: LocoStructOffset(0x6A), LocoArrayLength(256), Browsable(false)] char_t[] ScenarioDetails, // this is a string
		[property: LocoStructOffset(0x16A)] S5Header ScenarioText,
		[property: LocoStructOffset(0x17A)] uint16_t NumberOfForests,
		[property: LocoStructOffset(0x17C)] uint8_t MinForestRadius,
		[property: LocoStructOffset(0x17D)] uint8_t MaxForestRadius,
		[property: LocoStructOffset(0x17E)] uint8_t MinForestDensity,
		[property: LocoStructOffset(0x17F)] uint8_t MaxForestDensity,
		[property: LocoStructOffset(0x180)] uint16_t NumberRandomTrees,
		[property: LocoStructOffset(0x182)] uint8_t MinAltitudeForTrees,
		[property: LocoStructOffset(0x183)] uint8_t MaxAltitudeForTrees,
		[property: LocoStructOffset(0x184)] uint8_t MinLandHeight,
		[property: LocoStructOffset(0x185)] TopographyStyle TopographyStyle,
		[property: LocoStructOffset(0x186)] uint8_t HillDensity,
		[property: LocoStructOffset(0x187)] uint8_t NumberOfTowns,
		[property: LocoStructOffset(0x188)] uint8_t MaxTownSize,
		[property: LocoStructOffset(0x189)] uint8_t NumberOfIndustries,
		[property: LocoStructOffset(0x18A), LocoArrayLength(128 * 128), Browsable(false)] uint8_t[] Preview,
		[property: LocoStructOffset(0x418A)] uint8_t MaxCompetingCompanies,
		[property: LocoStructOffset(0x418B)] uint8_t CompetitorStartDelay,
		[property: LocoStructOffset(0x418C)] ScenarioObjective Objective,
		[property: LocoStructOffset(0x419D)] S5Header ObjectiveDeliveredCargo,
		[property: LocoStructOffset(0x41AD)] S5Header Currency,
		// openloco-specific after this
		[property: LocoStructOffset(0x41BD)] LandGeneratorType Generator,
		[property: LocoStructOffset(0x41BE)] uint8_t NumTerrainSmoothingPasses,
		[property: LocoStructOffset(0x41BF)] uint8_t NumRiverbeds,
		[property: LocoStructOffset(0x41C0)] uint8_t MinRiverWidth,
		[property: LocoStructOffset(0x41C1)] uint8_t MaxRiverWidth,
		[property: LocoStructOffset(0x41C2)] uint8_t RiverbankWidth,
		[property: LocoStructOffset(0x41C3)] uint8_t RiverMeanderRate,
		[property: LocoStructOffset(0x41C4), LocoArrayLength(342), Browsable(false)] byte[] var_41C4) : ILocoStruct
	{
		public const int StructLength = 0x431A;
		public bool Validate() => true;
	}
}
