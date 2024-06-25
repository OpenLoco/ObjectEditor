using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;

namespace Core.Types.SCV5
{
	[LocoStructSize(0x4A0644)] // 4,851,268
	class GameState
	{
		[LocoArrayLength(2)] uint32_t[] rng { get; set; }                                                                 // 0x000000 (0x00525E18)
		[LocoArrayLength(2)] uint32_t[] unkRng { get; set; }                                                              // 0x000008 (0x00525E20)
		uint32_t gameStateFlags { get; set; }                                                         // 0x000010 (0x00525E28) type is "GameStateFlags" in openloco
		uint32_t currentDay { get; set; }                                                             // 0x000014 (0x00525E2C)
		uint16_t dayCounter { get; set; }                                                             // 0x000018
		uint16_t currentYear { get; set; }                                                            // 0x00001A
		uint8_t currentMonth { get; set; }                                                            // 0x00001C
		uint8_t currentDayOfMonth { get; set; }                                                       // 0x00001D
		int16_t savedViewX { get; set; }                                                              // 0x00001E
		int16_t savedViewY { get; set; }                                                              // 0x000020
		uint8_t savedViewZoom { get; set; }                                                           // 0x000022
		uint8_t savedViewRotation { get; set; }                                                       // 0x000023
		[LocoArrayLength(2)] uint8_t[] playerCompanies { get; set; }                                                      // 0x000024 (0x00525E3C)
		[LocoArrayLength((int)Limits.kNumEntityLists)] uint16_t[] entityListHeads { get; set; }                               // 0x000026 (0x00525E3E)
		[LocoArrayLength((int)Limits.kNumEntityLists)] uint16_t[] entityListCounts { get; set; }                              // 0x000034 (0x00525E4C)
		[LocoArrayLength(0x046 - 0x042)] uint8_t[] pad_0042 { get; set; }                                                 // 0x000042
		[LocoArrayLength(32)] uint32_t[] currencyMultiplicationFactor { get; set; }                                       // 0x000046 (0x00525E5E)
		[LocoArrayLength(32)] uint32_t[] unusedCurrencyMultiplicationFactor { get; set; }                                 // 0x0000C6 (0x00525EDE)
		uint32_t scenarioTicks { get; set; }                                                          // 0x000146 (0x00525F5E)
		uint16_t var_014A { get; set; }                                                               // 0x00014A (0x00525F62)
		uint32_t scenarioTicks2 { get; set; }                                                         // 0x00014C (0x00525F64)
		uint32_t magicNumber { get; set; }                                                            // 0x000150 (0x00525F68)
		uint16_t numMapAnimations { get; set; }                                                       // 0x000154 (0x00525F6C)
		[LocoArrayLength(2)] int16_t[] tileUpdateStartLocation { get; set; }                                              // 0x000156 (0x00525F6E)
		[LocoArrayLength(8)] uint8_t[] scenarioSignals { get; set; }                                                      // 0x00015A (0x00525F72)
		[LocoArrayLength(8)] uint8_t[] scenarioBridges { get; set; }                                                      // 0x000162 (0x00525F7A)
		[LocoArrayLength(8)] uint8_t[] scenarioTrainStations { get; set; }                                                // 0x00016A (0x00525F82)
		[LocoArrayLength(8)] uint8_t[] scenarioTrackMods { get; set; }                                                    // 0x000172 (0x00525F8A)
		[LocoArrayLength(8)] uint8_t[] var_17A { get; set; }                                                              // 0x00017A (0x00525F92)
		[LocoArrayLength(8)] uint8_t[] scenarioRoadStations { get; set; }                                                 // 0x000182 (0x00525F9A)
		[LocoArrayLength(8)] uint8_t[] scenarioRoadMods { get; set; }                                                     // 0x00018A (0x00525FA2)
		uint8_t lastRailroadOption { get; set; }                                                      // 0x000192 (0x00525FAA)
		uint8_t lastRoadOption { get; set; }                                                          // 0x000193 (0x00525FAB)
		uint8_t lastAirport { get; set; }                                                             // 0x000194 (0x00525FAC)
		uint8_t lastShipPort { get; set; }                                                            // 0x000195 (0x00525FAD)
		bool trafficHandedness { get; set; }                                                          // 0x000196 (0x00525FAE)
		uint8_t lastVehicleType { get; set; }                                                         // 0x000197 (0x00525FAF)
		uint8_t pickupDirection { get; set; }                                                         // 0x000198 (0x00525FB0)
		uint8_t lastTreeOption { get; set; }                                                          // 0x000199 (0x00525FB1)
		uint16_t seaLevel { get; set; }                                                               // 0x00019A (0x00525FB2)
		uint8_t currentSnowLine { get; set; }                                                         // 0x00019C (0x00525FB4)
		uint8_t currentSeason { get; set; }                                                           // 0x00019D (0x00525FB5)
		uint8_t lastLandOption { get; set; }                                                          // 0x00019E (0x00525FB6)
		uint8_t maxCompetingCompanies { get; set; }                                                   // 0x00019F (0x00525FB7)
		uint32_t orderTableLength { get; set; }                                                       // 0x0001A0 (0x00525FB8)
		uint32_t roadObjectIdIsTram { get; set; }                                                     // 0x0001A4 (0x00525FBC)
		uint32_t roadObjectIdIsFlag7 { get; set; }                                                    // 0x0001A8 (0x00525FC0)
		uint8_t currentDefaultLevelCrossingType { get; set; }                                         // 0x0001AC (0x00525FC4)
		uint8_t lastTrackTypeOption { get; set; }                                                     // 0x0001AD (0x00525FC5)
		uint8_t loanInterestRate { get; set; }                                                        // 0x0001AE (0x00525FC6)
		uint8_t lastIndustryOption { get; set; }                                                      // 0x0001AF (0x00525FC7)
		uint8_t lastBuildingOption { get; set; }                                                      // 0x0001B0 (0x00525FC8)
		uint8_t lastMiscBuildingOption { get; set; }                                                  // 0x0001B1 (0x00525FC9)
		uint8_t lastWallOption { get; set; }                                                          // 0x0001B2 (0x00525FCA)
		uint8_t produceAICompanyTimeout { get; set; }                                                 // 0x0001B3 (0x00525FCB)
		[LocoArrayLength(2)] uint32_t[] tickStartPrngState { get; set; }                                                  // 0x0001B4 (0x00525FCC)
		[LocoArrayLength(256)] char[] scenarioFileName { get; set; }                                                      // 0x0001BC (0x00525FD4)
		[LocoArrayLength(64)] char[] scenarioName { get; set; }                                                           // 0x0002BC (0x005260D4)
		[LocoArrayLength(256)] char[] scenarioDetails { get; set; }                                                       // 0x0002FC (0x00526114)
		uint8_t competitorStartDelay { get; set; }                                                    // 0x0003FC (0x00526214)
		uint8_t preferredAIIntelligence { get; set; }                                                 // 0x0003FD (0x00526215)
		uint8_t preferredAIAggressiveness { get; set; }                                               // 0x0003FE (0x00526216)
		uint8_t preferredAICompetitiveness { get; set; }                                              // 0x0003FF (0x00526217)
		uint16_t startingLoanSize { get; set; }                                                       // 0x000400 (0x00526218)
		uint16_t maxLoanSize { get; set; }                                                            // 0x000402 (0x0052621A)
		uint32_t var_404 { get; set; }                                                                // 0x000404 (0x0052621C)
		uint32_t var_408 { get; set; }                                                                // 0x000408 (0x00526220)
		uint32_t var_40C { get; set; }                                                                // 0x00040C (0x00526224)
		uint32_t var_410 { get; set; }                                                                // 0x000410 (0x00526228)
		uint8_t lastBuildVehiclesOption { get; set; }                                                 // 0x000414 (0x0052622C)
		uint8_t numberOfIndustries { get; set; }                                                      // 0x000415 (0x0052622D)
		uint16_t vehiclePreviewRotationFrame { get; set; }                                            // 0x000416 (0x0052622E)
		uint8_t objectiveType { get; set; }                                                           // 0x000418 (0x00526230)
		uint8_t objectiveFlags { get; set; }                                                          // 0x000419 (0x00526231)
		uint32_t objectiveCompanyValue { get; set; }                                                  // 0x00041A (0x00526232)
		uint32_t objectiveMonthlyVehicleProfit { get; set; }                                          // 0x00041E (0x00526236)
		uint8_t objectivePerformanceIndex { get; set; }                                               // 0x000422 (0x0052623A)
		uint8_t objectiveDeliveredCargoType { get; set; }                                             // 0x000423 (0x0052623B)
		uint32_t objectiveDeliveredCargoAmount { get; set; }                                          // 0x000424 (0x0052623C)
		uint8_t objectiveTimeLimitYears { get; set; }                                                 // 0x000428 (0x00526240)
		uint16_t objectiveTimeLimitUntilYear { get; set; }                                            // 0x000429 (0x00526241)
		uint16_t objectiveMonthsInChallenge { get; set; }                                             // 0x00042B (0x00526243)
		uint16_t objectiveCompletedChallengeInMonths { get; set; }                                    // 0x00042D (0x00526245)
		uint8_t industryFlags { get; set; }                                                           // 0x00042F (0x00526247)
		uint16_t forbiddenVehiclesPlayers { get; set; }                                               // 0x000430 (0x00526248)
		uint16_t forbiddenVehiclesCompetitors { get; set; }                                           // 0x000432 (0x0052624A)
		S5FixFlags fixFlags { get; set; }                                                             // 0x000434 (0x0052624C)
		[LocoArrayLength(3)] uint16_t[] recordSpeed { get; set; }                                                         // 0x000436 (0x0052624E)
		[LocoArrayLength(4)] uint8_t[] recordCompany { get; set; }                                                        // 0x00043C (0x00526254)
		[LocoArrayLength(3)] uint32_t[] recordDate { get; set; }                                                          // 0x000440 (0x00526258)
		uint32_t var_44C { get; set; }                                                                // 0x00044C (0x00526264)
		uint32_t var_450 { get; set; }                                                                // 0x000450 (0x00526268)
		uint32_t var_454 { get; set; }                                                                // 0x000454 (0x0052626C)
		uint32_t var_458 { get; set; }                                                                // 0x000458 (0x00526270)
		uint32_t var_45C { get; set; }                                                                // 0x00045C (0x00526274)
		uint32_t var_460 { get; set; }                                                                // 0x000460 (0x00526278)
		uint32_t var_464 { get; set; }                                                                // 0x000464 (0x0052627C)
		uint32_t var_468 { get; set; }                                                                // 0x000468 (0x00526280)
		uint32_t lastMapWindowFlags { get; set; }                                                     // 0x00046C (0x00526284)
		[LocoArrayLength(2)] uint16_t[] lastMapWindowSize { get; set; }                                                   // 0x000470 (0x00526288)
		uint16_t lastMapWindowVar88A { get; set; }                                                    // 0x000474 (0x0052628C)
		uint16_t lastMapWindowVar88C { get; set; }                                                    // 0x000476 (0x0052628E)
		uint32_t var_478 { get; set; }                                                                // 0x000478 (0x00526290)
		[LocoArrayLength(0x13B6 - 0x47C)] uint8_t[] pad_047C { get; set; }                                                // 0x00047C
		uint16_t numMessages { get; set; }                                                            // 0x0013B6 (0x005271CE)
		uint16_t activeMessageIndex { get; set; }                                                     // 0x0013B8 (0x005271D0)
		[LocoArrayLength((int)Limits.kMaxMessages)] Message[] messages { get; set; }                                      // 0x0013BA (0x005271D2)
		[LocoArrayLength(0xB94C - 0xB886)] uint8_t[] pad_B886 { get; set; }                                               // 0x00B886
		uint8_t var_B94C { get; set; }                                                                // 0x00B94C (0x00531774)
		[LocoArrayLength(0xB950 - 0xB94D)] uint8_t[] pad_B94D { get; set; }                                               // 0x00B94D
		uint8_t var_B950 { get; set; }                                                                // 0x00B950 (0x00531778)
		uint8_t pad_B951 { get; set; }                                                                // 0x00B951
		uint8_t var_B952 { get; set; }                                                                // 0x00B952 (0x0053177A)
		uint8_t pad_B953 { get; set; }                                                                // 0x00B953
		uint8_t var_B954 { get; set; }                                                                // 0x00B954 (0x0053177C)
		uint8_t pad_B955 { get; set; }                                                                // 0x00B955
		uint8_t var_B956 { get; set; }                                                                // 0x00B956 (0x0053177E)
		[LocoArrayLength(0xB968 - 0xB957)] uint8_t[] pad_B957 { get; set; }                                               // 0x00B957
		uint8_t currentRainLevel { get; set; }                                                        // 0x00B968 (0x00531780)
		[LocoArrayLength(0xB96C - 0xB969)] uint8_t[] pad_B969 { get; set; }                                               // 0x00B969
		[LocoArrayLength((int)Limits.kMaxCompanies)] Company[] Companies { get; set; }                                    // 0x00B96C (0x00531784)
		[LocoArrayLength((int)Limits.kMaxTowns)] Town[] Towns { get; set; }                                               // 0x092444 (0x005B825C)
		[LocoArrayLength((int)Limits.kMaxIndustries)] Industry[] Industries { get; set; }                                 // 0x09E744 (0x005C455C)
		[LocoArrayLength((int)Limits.kMaxStations)] Station[] Stations { get; set; }                                      // 0x0C10C4 (0x005E6EDC)
		[LocoArrayLength((int)Limits.kMaxEntities)] Entity[] Entities { get; set; }                                       // 0x1B58C4 (0x006DB6DC)
		[LocoArrayLength((int)Limits.kMaxAnimations)] Animation[] Animations { get; set; }                                // 0x4268C4 (0x0094C6DC)
		[LocoArrayLength((int)Limits.kMaxWaves)] Wave[] Waves { get; set; }                                               // 0x4328C4 (0x009586DC)
																														  //[LocoArrayLength(Limits.kMaxUserStrings)] char[] UserStrings[32] { get; set; }                               // 0x432A44 (0x0095885C)
																														  //[LocoArrayLength(Limits.kMaxVehicles)] uint16_t[] Routings[Limits.kMaxRoutingsPerVehicle] { get; set; } // 0x442A44 (0x0096885C)
		[LocoArrayLength((int)Limits.kMaxOrders)] uint8_t[] Orders { get; set; }                                          // 0x461E44 (0x00987C5C)
	}
}
