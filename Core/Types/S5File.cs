using OpenLoco.ObjectEditor.Data;
using OpenLoco.ObjectEditor.DatFileParsing;
using OpenLoco.ObjectEditor.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.Types
{
	enum S5Type : uint8_t
	{
		SavedGame = 0,
		Scenario = 1,
		Objects = 2,
		Kandscape = 3,
	}

	[Flags]
	enum HeaderFlags : uint8_t
	{
		None = (byte)0U,
		IsRaw = (byte)1U << 0,
		IsDump = (byte)1U << 1,
		IsTitleSequence = (byte)1U << 2,
		HasSaveDetails = (byte)1U << 3,
	}

	[LocoStructSize(0x20)]
	class Header
	{
		S5Type Type { get; set; }
		HeaderFlags Flags { get; set; }
		uint16_t NumPackedObjects { get; set; }
		uint32_t Version { get; set; }
		uint32_t Magic { get; set; }
		[LocoArrayLength(20)]
		byte[] Padding { get; set; }
	}

	enum TopographyStyle : uint8_t
	{
		FlatLand,
		SmallHills,
		Mountains,
		HalfMountainsHills,
		HalfMountainsFlat,
	}

	enum LandGeneratorType : uint8_t
	{
		Original,
		Simplex,
		PngHeightMap,
	}

	enum LandDistributionPattern : uint8_t
	{
		Everywhere,
		Nowhere,
		FarFromWater,
		NearWater,
		OnMountains,
		FarFromMountains,
		InSmallRandomAreas,
		InLargeRandomAreas,
		AroundCliffs,
	}

	enum EditorControllerStep : int8_t
	{
		Null = -1,
		ObjectSelection = 0,
		LandscapeEditor = 1,
		ScenarioOptions = 2,
		SaveScenario = 3,
	}

	[Flags]
	enum ObjectiveFlags : uint8_t
	{
		None = (byte)0U,
		BeTopCompany = (byte)(1U << 0),
		BeWithinTopThreeCompanies = (byte)(1U << 1),
		WithinTimeLimit = (byte)(1U << 2),
		Flag_3 = (byte)(1U << 3),
		Flag_4 = (byte)(1U << 4),
	}

	enum ObjectiveType : uint8_t
	{
		CompanyValue,
		VehicleProfit,
		PerformanceIndex,
		CargoDelivery,
	}

	class ScenarioObjective
	{
		public ObjectiveType type { get; set; }   // 0x000418 (0x00526230)
		public ObjectiveFlags flags { get; set; } // 0x000419 (0x00526231)
		public uint32_t companyValue { get; set; }          // 0x00041A (0x00526232)
		public uint32_t monthlyVehicleProfit { get; set; }  // 0x00041E (0x00526236)
		public uint8_t performanceIndex { get; set; }       // 0x000422 (0x0052623A)
		public uint8_t deliveredCargoType { get; set; }     // 0x000423 (0x0052623B)
		public uint32_t deliveredCargoAmount { get; set; }  // 0x000424 (0x0052623C)
		public uint8_t timeLimitYears { get; set; }         // 0x000428 (0x00526240)
	}

	[Flags]
	enum ScenarioFlags : uint16_t
	{
		None = (byte)0U,
		LandscapeGenerationDone = (byte)(1U << 0),
		HillsEdgeOfMap = (byte)(1U << 1),
	}

	[LocoStructSize(0x431A)]
	class Options
	{
		public EditorControllerStep EditorStep { get; set; }                      // 0x00
		public uint8_t Difficulty { get; set; }                                   // 0x01
		public uint16_t ScenarioStartYear { get; set; }                           // 0x02
		[LocoArrayLength(2)]
		public uint8_t[] pad_4 { get; set; }                                     // 0x04
		public ScenarioFlags ScenarioFlags { get; set; }                          // 0x06
		public uint8_t MadeAnyChanges { get; set; }                               // 0x08
		public uint8_t pad_9 { get; set; }                                     // 0x09
		[LocoArrayLength(32)]
		public LandDistributionPattern LandDistributionPatterns { get; set; } // 0x0A
		[LocoArrayLength(64)]
		public char[] ScenarioName { get; set; }                                // 0x2A
		[LocoArrayLength(256)]
		public char[] ScenarioDetails { get; set; }                            // 0x6A
		public ObjectHeader ScenarioText { get; set; }                            // 0x16A
		public uint16_t NumberOfForests { get; set; }                             // 0x17a
		public uint8_t MinForestRadius { get; set; }                              // 0x17C
		public uint8_t MaxForestRadius { get; set; }                              // 0x17D
		public uint8_t MinForestDensity { get; set; }                             // 0x17E
		public uint8_t MaxForestDensity { get; set; }                             // 0x17F
		public uint16_t NumberRandomTrees { get; set; }                           // 0x180
		public uint8_t MinAltitudeForTrees { get; set; }                          // 0x182
		public uint8_t MaxAltitudeForTrees { get; set; }                          // 0x183
		public uint8_t MinLandHeight { get; set; }                                // 0x184
		public TopographyStyle TopographyStyle { get; set; }                      // 0x185
		public uint8_t HillDensity { get; set; }                                  // 0x186
		public uint8_t NumberOfTowns { get; set; }                                // 0x187
		public uint8_t MaxTownSize { get; set; }                                  // 0x188
		public uint8_t NumberOfIndustries { get; set; }                           // 0x189
		[LocoArrayLength(128 * 128)] // this is a 2d array
		public uint8_t[] Preview { get; set; }                            // 0x18A
		public uint8_t MaxCompetingCompanies { get; set; }                        // 0x418A
		public uint8_t CompetitorStartDelay { get; set; }                         // 0x418B
		public ScenarioObjective Objective { get; set; }                        // 0x418C
		public ObjectHeader ObjectiveDeliveredCargo { get; set; }                 // 0x419D
		public ObjectHeader Currency { get; set; }                                // 0x41AD

		// new fields:
		public LandGeneratorType Generator { get; set; }
		public uint8_t NumTerrainSmoothingPasses;
		[LocoArrayLength(347)]
		public byte[] pad_41BD { get; set; }
	}

	[Flags]
	enum CompanyFlags : uint32_t
	{
		None = 0U,
		Unk0 = 1U << 0,                      // 0x01
		Unk1 = 1U << 1,                      // 0x02
		Unk2 = 1U << 2,                      // 0x04
		Sorted = 1U << 3,                    // 0x08
		IncreasedPerformance = 1U << 4,      // 0x10
		DecreasedPerformance = 1U << 5,      // 0x20
		ChallengeCompleted = 1U << 6,        // 0x40
		ChallengeFailed = 1U << 7,           // 0x80
		ChallengeBeatenByOpponent = 1U << 8, // 0x100
		Bankrupt = 1U << 9,                  // 0x200
		AutopayLoan = 1U << 31,              // 0x80000000 new for OpenLoco
	}

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

	[Flags]
	enum S5FixFlags : uint16_t
	{
		none = (uint16_t)0U,
		fixFlag0 = (uint16_t)1U << 0,
		fixFlag1 = (uint16_t)1U << 1,
	};

	[LocoStructSize(0x8FA8)]
	class Company
	{
		uint16_t Name;                 // 0x0000
		uint16_t OwnerName;            // 0x0002
		CompanyFlags ChallengeFlags;   // 0x0004
		[LocoArrayLength(6)] uint8_t[] Cash;               // 0x0008
		uint32_t CurrentLoan;          // 0x000E
		uint32_t UpdateCounter;        // 0x0012
		int16_t PerformanceIndex;      // 0x0016
		[LocoArrayLength(0x8C4E - 0x18)] uint8_t[] pad_18; // 0x0018
		uint8_t ChallengeProgress;     // 0x8C4E
		[LocoArrayLength(0x8FA8 - 0x8C4F)] uint8_t[] pad_8C4F;
	}

	struct Town
	{
		[LocoArrayLength(0x270)] uint8_t[] pad_000;
	};

	struct Industry
	{
		[LocoArrayLength(0x453)] uint8_t[] pad_000;
	};

	struct Station
	{
		[LocoArrayLength(0x3D2)] uint8_t[] pad_000;
	};

	struct Entity
	{
		[LocoArrayLength(0x80)] uint8_t[] pad_00;
	};

	struct Animation
	{
		[LocoArrayLength(0x06)] uint8_t[] pad_0;
	};

	struct Wave
	{
		uint8_t pad_0[0x6];
	};

	struct Message
	{
		uint8_t pad_0[0xD4];
	};

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
		[LocoArrayLength(Limits.kNumEntityLists)] uint16_t[] entityListHeads { get; set; }                               // 0x000026 (0x00525E3E)
		[LocoArrayLength(Limits.kNumEntityLists)] uint16_t[] entityListCounts { get; set; }                              // 0x000034 (0x00525E4C)
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
		[LocoArrayLength(Limits.kMaxMessages)] Message[] messages { get; set; }                                      // 0x0013BA (0x005271D2)
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
		[LocoArrayLength(Limits.kMaxCompanies)] Company[] Companies { get; set; }                                    // 0x00B96C (0x00531784)
		[LocoArrayLength(Limits.kMaxTowns)] Town[] Towns { get; set; }                                               // 0x092444 (0x005B825C)
		[LocoArrayLength(Limits.kMaxIndustries)] Industry[] Industries { get; set; }                                 // 0x09E744 (0x005C455C)
		[LocoArrayLength(Limits.kMaxStations)] Station[] Stations { get; set; }                                      // 0x0C10C4 (0x005E6EDC)
		[LocoArrayLength(Limits.kMaxEntities)] Entity[] Entities { get; set; }                                       // 0x1B58C4 (0x006DB6DC)
		[LocoArrayLength(Limits.kMaxAnimations)] Animation[] Animations { get; set; }                                // 0x4268C4 (0x0094C6DC)
		[LocoArrayLength(Limits.kMaxWaves)] Wave[] Waves { get; set; }                                               // 0x4328C4 (0x009586DC)
		[LocoArrayLength(Limits.kMaxUserStrings)] char[] UserStrings[32] { get; set; }                               // 0x432A44 (0x0095885C)
		[LocoArrayLength(Limits.kMaxVehicles)] uint16_t[] Routings[Limits.kMaxRoutingsPerVehicle] { get; set; } // 0x442A44 (0x0096885C)
		[LocoArrayLength(Limits.kMaxOrders)] uint8_t[] Orders { get; set; }                                          // 0x461E44 (0x00987C5C)
	}

	[LocoStructSize(0x08)]
	class TileElement
	{
		public const uint8_t FLAG_GHOST = 1 << 4;
		public const uint8_t FLAG_LAST = 1 << 7;

		public uint8_t Type { get; set; }
		public uint8_t Flags { get; set; }
		public uint8_t BaseZ { get; set; }
		public uint8_t ClearZ { get; set; }
		[LocoArrayLength(4)]
		public uint8_t[] pad_4 { get; set; }

		void SetLast(bool value)
		{
			if (value)
				Flags |= FLAG_LAST;
			else
			{
				unchecked
				{
					Flags &= (byte)~FLAG_LAST;
				}
			}
		}

		bool IsGhost()
		{
			return (Flags & FLAG_GHOST) == FLAG_GHOST;
		}

		bool IsLast()
		{
			return (Flags & FLAG_LAST) == FLAG_LAST;
		}
	}

	class S5File
	{
		public Header Header { get; set; }
		public Options? LandscapeOptions { get; set; }
		public SaveDetails? SaveDetails { get; set; }

		// todo: make a list? is this harcoded?
		[LocoArrayLength(859)]
		public ObjectHeader[] RequiredObjects { get; set; }
		public GameState GameState { get; set; }
		public List<TileElement> TileElements { get; set; }
		public List<(ObjectHeader, byte[])> PackedObjects { get; set; }
	}
}
