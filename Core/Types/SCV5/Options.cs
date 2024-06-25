using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;

namespace Core.Types.SCV5
{
	[LocoStructSize(0x431A)]
	class Options
	{
		[LocoStructOffset(0x00)] public EditorControllerStep EditorStep { get; set; }
		[LocoStructOffset(0x01)] public uint8_t Difficulty { get; set; }
		[LocoStructOffset(0x02)] public uint16_t ScevnarioStartYear { get; set; }
		[LocoStructOffset(0x04), LocoArrayLength(2)] public uint8_t[] pad_4 { get; set; }
		[LocoStructOffset(0x06)] public ScenarioFlags ScenarioFlags { get; set; }
		[LocoStructOffset(0x08)] public uint8_t MadeAnyChanges { get; set; }
		[LocoStructOffset(0x09)] public uint8_t pad_9 { get; set; }
		[LocoStructOffset(0x0A), LocoArrayLength(32)] public LandDistributionPattern LandDistributionPatterns { get; set; }
		[LocoStructOffset(0x2A), LocoArrayLength(64)] public char[] ScenarioName { get; set; }
		[LocoStructOffset(0x6A), LocoArrayLength(256)] public char[] ScenarioDetails { get; set; }
		[LocoStructOffset(0x16A)] public ObjectHeader ScenarioText { get; set; }
		[LocoStructOffset(0x17A)] public uint16_t NumberOfForests { get; set; }
		[LocoStructOffset(0x17C)] public uint8_t MinForestRadius { get; set; }
		[LocoStructOffset(0x17D)] public uint8_t MaxForestRadius { get; set; }
		[LocoStructOffset(0x17E)] public uint8_t MinForestDensity { get; set; }
		[LocoStructOffset(0x17F)] public uint8_t MaxForestDensity { get; set; }
		[LocoStructOffset(0x180)] public uint16_t NumberRandomTrees { get; set; }
		[LocoStructOffset(0x182)] public uint8_t MinAltitudeForTrees { get; set; }
		[LocoStructOffset(0x183)] public uint8_t MaxAltitudeForTrees { get; set; }
		[LocoStructOffset(0x184)] public uint8_t MinLandHeight { get; set; }
		[LocoStructOffset(0x185)] public TopographyStyle TopographyStyle { get; set; }
		[LocoStructOffset(0x186)] public uint8_t HillDensity { get; set; }
		[LocoStructOffset(0x187)] public uint8_t NumberOfTowns { get; set; }
		[LocoStructOffset(0x188)] public uint8_t MaxTownSize { get; set; }
		[LocoStructOffset(0x189)] public uint8_t NumberOfIndustries { get; set; }
		[LocoStructOffset(0x18A), LocoArrayLength(128 * 128)] public uint8_t[] Preview { get; set; } // this is a 2D array
		[LocoStructOffset(0x418A)] public uint8_t MaxCompetingCompanies { get; set; }
		[LocoStructOffset(0x418B)] public uint8_t CompetitorStartDelay { get; set; }
		[LocoStructOffset(0x418C)] public ScenarioObjective Objective { get; set; }
		[LocoStructOffset(0x419D)] public ObjectHeader ObjectiveDeliveredCargo { get; set; }
		[LocoStructOffset(0x41AD)] public ObjectHeader Currency { get; set; }

		// new fields:
		[LocoStructOffset(0x41B2)] public LandGeneratorType Generator { get; set; }
		[LocoStructOffset(0x41B3)] public uint8_t NumTerrainSmoothingPasses { get; set; }
		[LocoStructOffset(0x41B4), LocoArrayLength(347)] public byte[] pad_41BD { get; set; }
	}
}
