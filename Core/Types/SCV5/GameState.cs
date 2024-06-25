using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	[LocoStructSize(0x4A0644)] // 4,851,268
	public class GameState
	{
		[LocoStructOffset(0x00), LocoArrayLength(2)] public uint32_t[] Rng { get; set; }
		[LocoStructOffset(0x08), LocoArrayLength(2)] public uint32_t[] UnkRng { get; set; }
		[LocoStructOffset(0x10)] public uint32_t GameStateFlags { get; set; } // 0x000010 (0x00525E28) type is "GameStateFlags" in openloco
		[LocoStructOffset(0x14)] public uint32_t CurrentDay { get; set; }
		[LocoStructOffset(0x18)] public uint16_t DayCounter { get; set; }
		[LocoStructOffset(0x1A)] public uint16_t CurrentYear { get; set; }
		[LocoStructOffset(0x1C)] public uint8_t CurrentMonth { get; set; }
		[LocoStructOffset(0x1D)] public uint8_t CurrentDayOfMonth { get; set; }
		[LocoStructOffset(0x1E)] public int16_t SavedViewX { get; set; }
		[LocoStructOffset(0x20)] public int16_t SavedViewY { get; set; }
		[LocoStructOffset(0x22)] public uint8_t SavedViewZoom { get; set; }
		[LocoStructOffset(0x23)] public uint8_t SavedViewRotation { get; set; }
		[LocoStructOffset(0x24), LocoArrayLength(2)] public uint8_t[] PlayerCompanies { get; set; }
		[LocoStructOffset(0x26), LocoArrayLength((int)Limits.kNumEntityLists)] public uint16_t[] EntityListHeads { get; set; }
		[LocoStructOffset(0x34), LocoArrayLength((int)Limits.kNumEntityLists)] public uint16_t[] EntityListCounts { get; set; }
		[LocoStructOffset(0x42), LocoArrayLength(0x046 - 0x042)] public uint8_t[] pad_0042 { get; set; }
		[LocoStructOffset(0x46), LocoArrayLength(32)] public uint32_t[] CurrencyMultiplicationFactor { get; set; }
		[LocoStructOffset(0xC6), LocoArrayLength(32)] public uint32_t[] UnusedCurrencyMultiplicationFactor { get; set; }
		[LocoStructOffset(0x146)] public uint32_t ScenarioTicks { get; set; }
		[LocoStructOffset(0x14A)] public uint16_t var_014A { get; set; }
		[LocoStructOffset(0x14C)] public uint32_t ScenarioTicks2 { get; set; }
		[LocoStructOffset(0x150)] public uint32_t MagicNumber { get; set; }
		[LocoStructOffset(0x154)] public uint16_t NumMapAnimations { get; set; }
		[LocoStructOffset(0x156), LocoArrayLength(2)] public int16_t[] TileUpdateStartLocation { get; set; }
		[LocoStructOffset(0x15A), LocoArrayLength(8)] public uint8_t[] ScenarioSignals { get; set; }
		[LocoStructOffset(0x162), LocoArrayLength(8)] public uint8_t[] ScenarioBridges { get; set; }
		[LocoStructOffset(0x16A), LocoArrayLength(8)] public uint8_t[] ScenarioTrainStations { get; set; }
		[LocoStructOffset(0x172), LocoArrayLength(8)] public uint8_t[] ScenarioTrackMods { get; set; }
		[LocoStructOffset(0x17A), LocoArrayLength(8)] public uint8_t[] Var_17A { get; set; }
		[LocoStructOffset(0x182), LocoArrayLength(8)] public uint8_t[] ScenarioRoadStations { get; set; }
		[LocoStructOffset(0x18A), LocoArrayLength(8)] public uint8_t[] ScenarioRoadMods { get; set; }
		[LocoStructOffset(0x192)] public uint8_t LastRailroadOption { get; set; }
		[LocoStructOffset(0x193)] public uint8_t LastRoadOption { get; set; }
		[LocoStructOffset(0x194)] public uint8_t LastAirport { get; set; }
		[LocoStructOffset(0x195)] public uint8_t LastShipPort { get; set; }
		[LocoStructOffset(0x196)] public bool TrafficHandedness { get; set; }
		[LocoStructOffset(0x197)] public uint8_t LastVehicleType { get; set; }
		[LocoStructOffset(0x198)] public uint8_t PickupDirection { get; set; }
		[LocoStructOffset(0x199)] public uint8_t LastTreeOption { get; set; }
		[LocoStructOffset(0x19A)] public uint16_t SeaLevel { get; set; }
		[LocoStructOffset(0x19C)] public uint8_t CurrentSnowLine { get; set; }
		[LocoStructOffset(0x19D)] public uint8_t CurrentSeason { get; set; }
		[LocoStructOffset(0x19E)] public uint8_t LastLandOption { get; set; }
		[LocoStructOffset(0x19F)] public uint8_t MaxCompetingCompanies { get; set; }
		[LocoStructOffset(0x1A0)] public uint32_t OrderTableLength { get; set; }
		[LocoStructOffset(0x1A4)] public uint32_t RoadObjectIdIsTram { get; set; }
		[LocoStructOffset(0x1A8)] public uint32_t RoadObjectIdIsFlag7 { get; set; }
		[LocoStructOffset(0x1AC)] public uint8_t CurrentDefaultLevelCrossingType { get; set; }
		[LocoStructOffset(0x1AD)] public uint8_t LastTrackTypeOption { get; set; }
		[LocoStructOffset(0x1AE)] public uint8_t LoanInterestRate { get; set; }
		[LocoStructOffset(0x1AF)] public uint8_t LastIndustryOption { get; set; }
		[LocoStructOffset(0x1B0)] public uint8_t LastBuildingOption { get; set; }
		[LocoStructOffset(0x1B1)] public uint8_t LastMiscBuildingOption { get; set; }
		[LocoStructOffset(0x1B2)] public uint8_t LastWallOption { get; set; }
		[LocoStructOffset(0x1B3)] public uint8_t ProduceAICompanyTimeout { get; set; }
		[LocoStructOffset(0x1B4), LocoArrayLength(2)] public uint32_t[] TickStartPrngState { get; set; }
		[LocoStructOffset(0x1BC), LocoArrayLength(256)] public char[] ScenarioFileName { get; set; }
		[LocoStructOffset(0x2BC), LocoArrayLength(64)] public char[] ScenarioName { get; set; }
		[LocoStructOffset(0x2FC), LocoArrayLength(256)] public char[] ScenarioDetails { get; set; }
		[LocoStructOffset(0x3FC)] public uint8_t CompetitorStartDelay { get; set; }
		[LocoStructOffset(0x3FD)] public uint8_t PreferredAIIntelligence { get; set; }
		[LocoStructOffset(0x3FE)] public uint8_t PreferredAIAggressiveness { get; set; }
		[LocoStructOffset(0x3FF)] public uint8_t PreferredAICompetitiveness { get; set; }
		[LocoStructOffset(0x400)] public uint16_t StartingLoanSize { get; set; }
		[LocoStructOffset(0x402)] public uint16_t MaxLoanSize { get; set; }
		[LocoStructOffset(0x404)] public uint32_t var_404 { get; set; }
		[LocoStructOffset(0x408)] public uint32_t var_408 { get; set; }
		[LocoStructOffset(0x40C)] public uint32_t var_40C { get; set; }
		[LocoStructOffset(0x410)] public uint32_t var_410 { get; set; }
		[LocoStructOffset(0x414)] public uint8_t LastBuildVehiclesOption { get; set; }
		[LocoStructOffset(0x415)] public uint8_t NumberOfIndustries { get; set; }
		[LocoStructOffset(0x416)] public uint16_t VehiclePreviewRotationFrame { get; set; }
		[LocoStructOffset(0x418)] public uint8_t ObjectiveType { get; set; }
		[LocoStructOffset(0x419)] public uint8_t ObjectiveFlags { get; set; }
		[LocoStructOffset(0x41A)] public uint32_t ObjectiveCompanyValue { get; set; }
		[LocoStructOffset(0x41E)] public uint32_t ObjectiveMonthlyVehicleProfit { get; set; }
		[LocoStructOffset(0x422)] public uint8_t ObjectivePerformanceIndex { get; set; }
		[LocoStructOffset(0x423)] public uint8_t ObjectiveDeliveredCargoType { get; set; }
		[LocoStructOffset(0x424)] public uint32_t ObjectiveDeliveredCargoAmount { get; set; }
		[LocoStructOffset(0x428)] public uint8_t ObjectiveTimeLimitYears { get; set; }
		[LocoStructOffset(0x429)] public uint16_t ObjectiveTimeLimitUntilYear { get; set; }
		[LocoStructOffset(0x42B)] public uint16_t ObjectiveMonthsInChallenge { get; set; }
		[LocoStructOffset(0x42D)] public uint16_t ObjectiveCompletedChallengeInMonths { get; set; }
		[LocoStructOffset(0x42F)] public uint8_t IndustryFlags { get; set; }
		[LocoStructOffset(0x430)] public uint16_t ForbiddenVehiclesPlayers { get; set; }
		[LocoStructOffset(0x432)] public uint16_t ForbiddenVehiclesCompetitors { get; set; }
		[LocoStructOffset(0x434)] public S5FixFlags FixFlags { get; set; }
		[LocoStructOffset(0x436), LocoArrayLength(3)] public uint16_t[] RecordSpeed { get; set; }
		[LocoStructOffset(0x43C), LocoArrayLength(4)] public uint8_t[] RecordCompany { get; set; }
		[LocoStructOffset(0x440), LocoArrayLength(3)] public uint32_t[] RecordDate { get; set; }
		[LocoStructOffset(0x44C)] public uint32_t var_44C { get; set; }
		[LocoStructOffset(0x450)] public uint32_t var_450 { get; set; }
		[LocoStructOffset(0x454)] public uint32_t var_454 { get; set; }
		[LocoStructOffset(0x458)] public uint32_t var_458 { get; set; }
		[LocoStructOffset(0x45C)] public uint32_t var_45C { get; set; }
		[LocoStructOffset(0x460)] public uint32_t var_460 { get; set; }
		[LocoStructOffset(0x464)] public uint32_t var_464 { get; set; }
		[LocoStructOffset(0x468)] public uint32_t var_468 { get; set; }
		[LocoStructOffset(0x46C)] public uint32_t LastMapWindowFlags { get; set; }
		[LocoStructOffset(0x470), LocoArrayLength(2)] public uint16_t[] LastMapWindowSize { get; set; }
		[LocoStructOffset(0x474)] public uint16_t LastMapWindowVar88A { get; set; }
		[LocoStructOffset(0x476)] public uint16_t LastMapWindowVar88C { get; set; }
		[LocoStructOffset(0x478)] public uint32_t var_478 { get; set; }
		[LocoStructOffset(0x47C), LocoArrayLength(0x13B6 - 0x47C)] public uint8_t[] pad_047C { get; set; }
		[LocoStructOffset(0x13B6)] public uint16_t NumMessages { get; set; }
		[LocoStructOffset(0x13B8)] public uint16_t ActiveMessageIndex { get; set; }
		[LocoStructOffset(0x13BA), LocoArrayLength((int)Limits.kMaxMessages)] public Message[] Messages { get; set; }
		[LocoStructOffset(0xB886), LocoArrayLength(0xB94C - 0xB886)] public uint8_t[] pad_B886 { get; set; }
		[LocoStructOffset(0xB94C)] public uint8_t var_B94C { get; set; }
		[LocoStructOffset(0xB94D), LocoArrayLength(0xB950 - 0xB94D)] public uint8_t[] pad_B94D { get; set; }
		[LocoStructOffset(0xB950)] public uint8_t var_B950 { get; set; }
		[LocoStructOffset(0xB951)] public uint8_t pad_B951 { get; set; }
		[LocoStructOffset(0xB952)] public uint8_t var_B952 { get; set; }
		[LocoStructOffset(0xB953)] public uint8_t pad_B953 { get; set; }
		[LocoStructOffset(0xB954)] public uint8_t var_B954 { get; set; }
		[LocoStructOffset(0xB955)] public uint8_t pad_B955 { get; set; }
		[LocoStructOffset(0xB956)] public uint8_t var_B956 { get; set; }
		[LocoStructOffset(0xB957), LocoArrayLength(0xB968 - 0xB957)] public uint8_t[] pad_B957 { get; set; }
		[LocoStructOffset(0xB958)] public uint8_t CurrentRainLevel { get; set; }
		[LocoStructOffset(0xB959), LocoArrayLength(0xB96C - 0xB969)] public uint8_t[] pad_B969 { get; set; }
		[LocoStructOffset(0xB96C), LocoArrayLength((int)Limits.kMaxCompanies)] public Company[] Companies { get; set; }
		[LocoStructOffset(0x92444), LocoArrayLength((int)Limits.kMaxTowns)] public Town[] Towns { get; set; }
		[LocoStructOffset(0x9E744), LocoArrayLength((int)Limits.kMaxIndustries)] public Industry[] Industries { get; set; }
		[LocoStructOffset(0xC10C4), LocoArrayLength((int)Limits.kMaxStations)] public Station[] Stations { get; set; }
		[LocoStructOffset(0x1B58C4), LocoArrayLength((int)Limits.kMaxEntities)] public Entity[] Entities { get; set; }
		[LocoStructOffset(0x4268C4), LocoArrayLength((int)Limits.kMaxAnimations)] public Animation[] Animations { get; set; }
		[LocoStructOffset(0x4328C4), LocoArrayLength((int)Limits.kMaxWaves)] public Wave[] Waves { get; set; }
		//[LocoStructOffset(0x432A44), LocoArrayLength(Limits.kMaxUserStrings)] char[] UserStrings[32] { get; set; }
		[LocoStructOffset(0x432A44), LocoArrayLength((int)Limits.kMaxUserStrings * 32)] public char[] UserStrings { get; set; }
		//[LocoStructOffset(0x442A44), LocoArrayLength(Limits.kMaxVehicles)] uint16_t[] Routings[Limits.kMaxRoutingsPerVehicle] { get; set; }
		[LocoStructOffset(0x442A44), LocoArrayLength((int)Limits.kMaxVehicles * (int)Limits.kMaxRoutingsPerVehicle)] public uint16_t[] Routings { get; set; }
		[LocoStructOffset(0x461E44), LocoArrayLength((int)Limits.kMaxOrders)] public uint8_t[] Orders { get; set; }
	}
}
