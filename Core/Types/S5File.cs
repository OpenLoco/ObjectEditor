using OpenLoco.ObjectEditor.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Types
{
	enum S5Type : uint8_t
	{
		SavedGame = 0,
		Scenario = 1,
		Objects = 2,
		Kandscape = 3,
	};

	enum HeaderFlags : uint8_t
	{
		None = (byte)0U,
		IsRaw = (byte)1U << 0,
		IsDump = (byte)1U << 1,
		IsTitleSequence = (byte)1U << 2,
		HasSaveDetails = (byte)1U << 3,
	};

	class Header // size 0x29
	{
		S5Type type;
		HeaderFlags flags;
		uint16_t numPackedObjects;
		uint32_t version;
		uint32_t magic;
		byte[] padding; // exactly 20 bytes
	}

	enum TopographyStyle : uint8_t
	{
		FlatLand,
		SmallHills,
		Mountains,
		HalfMountainsHills,
		HalfMountainsFlat,
	};

	enum LandGeneratorType : uint8_t
	{
		Original,
		Simplex,
		PngHeightMap,
	};

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
	};

	enum EditorControllerStep : int8_t
	{
		Null = -1,
		ObjectSelection = 0,
		LandscapeEditor = 1,
		ScenarioOptions = 2,
		SaveScenario = 3,
	};

	enum ObjectiveFlags : uint8_t
	{
		None = (byte)0U,
		BeTopCompany = (byte)(1U << 0),
		BeWithinTopThreeCompanies = (byte)(1U << 1),
		WithinTimeLimit = (byte)(1U << 2),
		Flag_3 = (byte)(1U << 3),
		Flag_4 = (byte)(1U << 4),
	};

	enum ObjectiveType : uint8_t
	{
		CompanyValue,
		VehicleProfit,
		PerformanceIndex,
		CargoDelivery,
	};

	struct ScenarioObjective
	{
		ObjectiveType type;   // 0x000418 (0x00526230)
		ObjectiveFlags flags; // 0x000419 (0x00526231)
		uint32_t companyValue;          // 0x00041A (0x00526232)
		uint32_t monthlyVehicleProfit;  // 0x00041E (0x00526236)
		uint8_t performanceIndex;       // 0x000422 (0x0052623A)
		uint8_t deliveredCargoType;     // 0x000423 (0x0052623B)
		uint32_t deliveredCargoAmount;  // 0x000424 (0x0052623C)
		uint8_t timeLimitYears;         // 0x000428 (0x00526240)
	};

	enum ScenarioFlags : uint16_t
	{
		None = (byte)0U,
		LandscapeGenerationDone = (byte)(1U << 0),
		HillsEdgeOfMap = (byte)(1U << 1),
	};

	class Options
	{
		EditorControllerStep editorStep;                      // 0x00
		uint8_t difficulty;                                   // 0x01
		uint16_t scenarioStartYear;                           // 0x02
		uint8_t pad_4[2];                                     // 0x04
		ScenarioFlags scenarioFlags;                          // 0x06
		uint8_t madeAnyChanges;                               // 0x08
		uint8_t pad_9[1];                                     // 0x09
		LandDistributionPattern landDistributionPatterns[32]; // 0x0A
		char scenarioName[64];                                // 0x2A
		char scenarioDetails[256];                            // 0x6A
		ObjectHeader scenarioText;                            // 0x16A
		uint16_t numberOfForests;                             // 0x17a
		uint8_t minForestRadius;                              // 0x17C
		uint8_t maxForestRadius;                              // 0x17D
		uint8_t minForestDensity;                             // 0x17E
		uint8_t maxForestDensity;                             // 0x17F
		uint16_t numberRandomTrees;                           // 0x180
		uint8_t minAltitudeForTrees;                          // 0x182
		uint8_t maxAltitudeForTrees;                          // 0x183
		uint8_t minLandHeight;                                // 0x184
		TopographyStyle topographyStyle;                      // 0x185
		uint8_t hillDensity;                                  // 0x186
		uint8_t numberOfTowns;                                // 0x187
		uint8_t maxTownSize;                                  // 0x188
		uint8_t numberOfIndustries;                           // 0x189
		uint8_t preview[128][128];                            // 0x18A
        uint8_t maxCompetingCompanies;                        // 0x418A
		uint8_t competitorStartDelay;                         // 0x418B
		ScenarioObjective objective;                        // 0x418C
		ObjectHeader objectiveDeliveredCargo;                 // 0x419D
		ObjectHeader currency;                                // 0x41AD

		// new fields:
		LandGeneratorType generator;
		uint8_t numTerrainSmoothingPasses;

		byte pad_41BD[347];
	}

	enum CompanyFlags : uint32_t
	{
		None = 0U,
		Unk0 = (1U << 0),                      // 0x01
		Unk1 = (1U << 1),                      // 0x02
		Unk2 = (1U << 2),                      // 0x04
		Sorted = (1U << 3),                    // 0x08
		IncreasedPerformance = (1U << 4),      // 0x10
		DecreasedPerformance = (1U << 5),      // 0x20
		ChallengeCompleted = (1U << 6),        // 0x40
		ChallengeFailed = (1U << 7),           // 0x80
		ChallengeBeatenByOpponent = (1U << 8), // 0x100
		Bankrupt = (1U << 9),                  // 0x200
		AutopayLoan = (1U << 31),              // 0x80000000 new for OpenLoco
	};

	class SaveDetails
	{
		char company[256];                   // 0x000
		char owner[256];                     // 0x100
		uint32_t date;                       // 0x200
		uint16_t performanceIndex;           // 0x204 (from [company.performance_index)
		char scenario[0x40];                 // 0x206
		uint8_t challengeProgress;           // 0x246
		byte pad_247;                   // 0x247
		uint8_t image[250 * 200];            // 0x248
		CompanyFlags challengeFlags;         // 0xC598 (from [company.challenge_flags])
		byte pad_C59C[0xC618 - 0xC59C]; // 0xC59C
	}

	class GameState
	{ }

	// size 0x8
	class TileElement
	{
		const uint8_t FLAG_GHOST = 1 << 4;
		const uint8_t FLAG_LAST = 1 << 7;

		uint8_t type;
		uint8_t flags;
		uint8_t baseZ;
		uint8_t clearZ;
		uint8_t pad_4[4];

		void setLast(bool value)
		{
			if (value)
				flags |= FLAG_LAST;
			else
				flags &= ~FLAG_LAST;
		}

		bool isGhost()
		{
			return (flags & FLAG_GHOST) == FLAG_GHOST;
		}

		bool isLast()
		{
			return (flags & FLAG_LAST) == FLAG_LAST;
		}
	};

	class S5File
	{
		Header header;
		Options? landscapeOptions;
		SaveDetails? saveDetails;
		ObjectHeader requiredObjects[859];
		GameState gameState;
		List<TileElement> tileElements;
		List<(ObjectHeader, byte[])> packedObjects;
	};
}
