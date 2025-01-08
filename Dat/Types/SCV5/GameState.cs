using OpenLoco.Dat.Data;
using OpenLoco.Dat.FileParsing;
using System.ComponentModel;

namespace OpenLoco.Dat.Types.SCV5
{
	[Flags]
	public enum GameStateFlags : uint32_t
	{
		None = 0,
		TileManagerLoaded = 1 << 0, // true => tile elements exist, false => proc gen map
		Unk2 = 1 << 1,
		PreferredOwnerName = 1 << 2,
	}

	[TypeConverter(typeof(ExpandableObjectConverter))]
	[LocoStructSize(0xB96C)]
	public record GameStateA(
		[property: LocoStructOffset(0x00), LocoArrayLength(2)] uint32_t[] Rng,
		[property: LocoStructOffset(0x08), LocoArrayLength(2)] uint32_t[] UnkRng,
		[property: LocoStructOffset(0x10)] GameStateFlags GameStateFlags,
		[property: LocoStructOffset(0x14)] uint32_t CurrentDay,
		[property: LocoStructOffset(0x18)] uint16_t DayCounter,
		[property: LocoStructOffset(0x1A)] uint16_t CurrentYear,
		[property: LocoStructOffset(0x1C)] uint8_t CurrentMonth,
		[property: LocoStructOffset(0x1D)] uint8_t CurrentDayOfMonth,
		[property: LocoStructOffset(0x1E)] int16_t SavedViewX,
		[property: LocoStructOffset(0x20)] int16_t SavedViewY,
		[property: LocoStructOffset(0x22)] uint8_t SavedViewZoom,
		[property: LocoStructOffset(0x23)] uint8_t SavedViewRotation,
		[property: LocoStructOffset(0x24), LocoArrayLength(2)] uint8_t[] PlayerCompanies,
		[property: LocoStructOffset(0x26), LocoArrayLength((int)Limits.kNumEntityLists)] uint16_t[] EntityListHeads,
		[property: LocoStructOffset(0x34), LocoArrayLength((int)Limits.kNumEntityLists)] uint16_t[] EntityListCounts,
		[property: LocoStructOffset(0x42), LocoArrayLength(0x046 - 0x042)] uint8_t[] var_0042,
		[property: LocoStructOffset(0x46), LocoArrayLength(32)] uint32_t[] CurrencyMultiplicationFactor,
		[property: LocoStructOffset(0xC6), LocoArrayLength(32)] uint32_t[] UnusedCurrencyMultiplicationFactor,
		[property: LocoStructOffset(0x146)] uint32_t ScenarioTicks,
		[property: LocoStructOffset(0x14A)] uint16_t var_014A,
		[property: LocoStructOffset(0x14C)] uint32_t ScenarioTicks2,
		[property: LocoStructOffset(0x150)] uint32_t MagicNumber,
		[property: LocoStructOffset(0x154)] uint16_t NumMapAnimations,
		[property: LocoStructOffset(0x156), LocoArrayLength(2)] int16_t[] TileUpdateStartLocation,
		[property: LocoStructOffset(0x15A), LocoArrayLength(8)] uint8_t[] ScenarioSignals,
		[property: LocoStructOffset(0x162), LocoArrayLength(8)] uint8_t[] ScenarioBridges,
		[property: LocoStructOffset(0x16A), LocoArrayLength(8)] uint8_t[] ScenarioTrainStations,
		[property: LocoStructOffset(0x172), LocoArrayLength(8)] uint8_t[] ScenarioTrackMods,
		[property: LocoStructOffset(0x17A), LocoArrayLength(8)] uint8_t[] Var_17A,
		[property: LocoStructOffset(0x182), LocoArrayLength(8)] uint8_t[] ScenarioRoadStations,
		[property: LocoStructOffset(0x18A), LocoArrayLength(8)] uint8_t[] ScenarioRoadMods,
		[property: LocoStructOffset(0x192)] uint8_t LastRailroadOption,
		[property: LocoStructOffset(0x193)] uint8_t LastRoadOption,
		[property: LocoStructOffset(0x194)] uint8_t LastAirport,
		[property: LocoStructOffset(0x195)] uint8_t LastShipPort,
		[property: LocoStructOffset(0x196)] bool TrafficHandedness,
		[property: LocoStructOffset(0x197)] uint8_t LastVehicleType,
		[property: LocoStructOffset(0x198)] uint8_t PickupDirection,
		[property: LocoStructOffset(0x199)] uint8_t LastTreeOption,
		[property: LocoStructOffset(0x19A)] uint16_t SeaLevel,
		[property: LocoStructOffset(0x19C)] uint8_t CurrentSnowLine,
		[property: LocoStructOffset(0x19D)] uint8_t CurrentSeason,
		[property: LocoStructOffset(0x19E)] uint8_t LastLandOption,
		[property: LocoStructOffset(0x19F)] uint8_t MaxCompetingCompanies,
		[property: LocoStructOffset(0x1A0)] uint32_t OrderTableLength,
		[property: LocoStructOffset(0x1A4)] uint32_t RoadObjectIdIsTram,
		[property: LocoStructOffset(0x1A8)] uint32_t RoadObjectIdIsFlag7,
		[property: LocoStructOffset(0x1AC)] uint8_t CurrentDefaultLevelCrossingType,
		[property: LocoStructOffset(0x1AD)] uint8_t LastTrackTypeOption,
		[property: LocoStructOffset(0x1AE)] uint8_t LoanInterestRate,
		[property: LocoStructOffset(0x1AF)] uint8_t LastIndustryOption,
		[property: LocoStructOffset(0x1B0)] uint8_t LastBuildingOption,
		[property: LocoStructOffset(0x1B1)] uint8_t LastMiscBuildingOption,
		[property: LocoStructOffset(0x1B2)] uint8_t LastWallOption,
		[property: LocoStructOffset(0x1B3)] uint8_t ProduceAICompanyTimeout,
		[property: LocoStructOffset(0x1B4), LocoArrayLength(2)] uint32_t[] TickStartPrngState,
		[property: LocoStructOffset(0x1BC), LocoArrayLength(256)] char_t[] ScenarioFileName,
		[property: LocoStructOffset(0x2BC), LocoArrayLength(64)] char_t[] ScenarioName,
		[property: LocoStructOffset(0x2FC), LocoArrayLength(256)] char_t[] ScenarioDetails,
		[property: LocoStructOffset(0x3FC)] uint8_t CompetitorStartDelay,
		[property: LocoStructOffset(0x3FD)] uint8_t PreferredAIIntelligence,
		[property: LocoStructOffset(0x3FE)] uint8_t PreferredAIAggressiveness,
		[property: LocoStructOffset(0x3FF)] uint8_t PreferredAICompetitiveness,
		[property: LocoStructOffset(0x400)] uint16_t StartingLoanSize,
		[property: LocoStructOffset(0x402)] uint16_t MaxLoanSize,
		[property: LocoStructOffset(0x404)] uint32_t var_404,
		[property: LocoStructOffset(0x408)] uint32_t var_408,
		[property: LocoStructOffset(0x40C)] uint32_t var_40C,
		[property: LocoStructOffset(0x410)] uint32_t var_410,
		[property: LocoStructOffset(0x414)] uint8_t LastBuildVehiclesOption,
		[property: LocoStructOffset(0x415)] uint8_t NumberOfIndustries,
		[property: LocoStructOffset(0x416)] uint16_t VehiclePreviewRotationFrame,
		[property: LocoStructOffset(0x418)] uint8_t ObjectiveType,
		[property: LocoStructOffset(0x419)] uint8_t ObjectiveFlags,
		[property: LocoStructOffset(0x41A)] uint32_t ObjectiveCompanyValue,
		[property: LocoStructOffset(0x41E)] uint32_t ObjectiveMonthlyVehicleProfit,
		[property: LocoStructOffset(0x422)] uint8_t ObjectivePerformanceIndex,
		[property: LocoStructOffset(0x423)] uint8_t ObjectiveDeliveredCargoType,
		[property: LocoStructOffset(0x424)] uint32_t ObjectiveDeliveredCargoAmount,
		[property: LocoStructOffset(0x428)] uint8_t ObjectiveTimeLimitYears,
		[property: LocoStructOffset(0x429)] uint16_t ObjectiveTimeLimitUntilYear,
		[property: LocoStructOffset(0x42B)] uint16_t ObjectiveMonthsInChallenge,
		[property: LocoStructOffset(0x42D)] uint16_t ObjectiveCompletedChallengeInMonths,
		[property: LocoStructOffset(0x42F)] uint8_t IndustryFlags,
		[property: LocoStructOffset(0x430)] uint16_t ForbiddenVehiclesPlayers,
		[property: LocoStructOffset(0x432)] uint16_t ForbiddenVehiclesCompetitors,
		[property: LocoStructOffset(0x434)] S5FixFlags FixFlags,
		[property: LocoStructOffset(0x436), LocoArrayLength(3)] uint16_t[] RecordSpeed,
		[property: LocoStructOffset(0x43C), LocoArrayLength(4)] uint8_t[] RecordCompany,
		[property: LocoStructOffset(0x440), LocoArrayLength(3)] uint32_t[] RecordDate,
		[property: LocoStructOffset(0x44C)] uint32_t var_44C,
		[property: LocoStructOffset(0x450)] uint32_t var_450,
		[property: LocoStructOffset(0x454)] uint32_t var_454,
		[property: LocoStructOffset(0x458)] uint32_t var_458,
		[property: LocoStructOffset(0x45C)] uint32_t var_45C,
		[property: LocoStructOffset(0x460)] uint32_t var_460,
		[property: LocoStructOffset(0x464)] uint32_t var_464,
		[property: LocoStructOffset(0x468)] uint32_t var_468,
		[property: LocoStructOffset(0x46C)] uint32_t LastMapWindowFlags,
		[property: LocoStructOffset(0x470), LocoArrayLength(2)] uint16_t[] LastMapWindowSize,
		[property: LocoStructOffset(0x474)] uint16_t LastMapWindowVar88A,
		[property: LocoStructOffset(0x476)] uint16_t LastMapWindowVar88C,
		[property: LocoStructOffset(0x478)] uint32_t var_478,
		[property: LocoStructOffset(0x47C), LocoArrayLength(0x13B6 - 0x47C)] uint8_t[] var_047C,
		[property: LocoStructOffset(0x13B6)] uint16_t NumMessages,
		[property: LocoStructOffset(0x13B8)] uint16_t ActiveMessageIndex,
		[property: LocoStructOffset(0x13BA), LocoArrayLength((int)Limits.kMaxMessages)] Message[] Messages,
		[property: LocoStructOffset(0xB886), LocoArrayLength(0xB94C - 0xB886)] uint8_t[] var_B886,
		[property: LocoStructOffset(0xB94C)] uint8_t var_B94C,
		[property: LocoStructOffset(0xB94D), LocoArrayLength(0xB950 - 0xB94D)] uint8_t[] var_B94D,
		[property: LocoStructOffset(0xB950)] uint8_t var_B950,
		[property: LocoStructOffset(0xB951)] uint8_t var_B951,
		[property: LocoStructOffset(0xB952)] uint8_t var_B952,
		[property: LocoStructOffset(0xB953)] uint8_t var_B953,
		[property: LocoStructOffset(0xB954)] uint8_t var_B954,
		[property: LocoStructOffset(0xB955)] uint8_t var_B955,
		[property: LocoStructOffset(0xB956)] uint8_t var_B956,
		[property: LocoStructOffset(0xB957), LocoArrayLength(0xB968 - 0xB957)] uint8_t[] var_B957,
		[property: LocoStructOffset(0xB958)] uint8_t CurrentRainLevel,
		[property: LocoStructOffset(0xB959), LocoArrayLength(0xB96C - 0xB969)] uint8_t[] var_B969
		//[property: LocoStructOffset(0xB96C), LocoArrayLength((int)Limits.kMaxCompanies)] Company[] Companies // this isn't actually part of the data chunk in a scenario!
		)
		: ILocoStruct
	{
		//public const int StructLength = 0x4A0644;

		public bool Validate() => true;
	}

	[LocoStructSize(0x123480)]
	public record GameStateB(

		[property: LocoStructOffset(0x0), LocoArrayLength((int)Limits.kMaxTowns)] Town[] Towns,
		[property: LocoStructOffset(0xC300), LocoArrayLength((int)Limits.kMaxIndustries)] Industry[] Industries,
		[property: LocoStructOffset(0x2EC80), LocoArrayLength((int)Limits.kMaxStations)] Station[] Stations
	//[property: LocoStructOffset(0x123480), LocoArrayLength((int)Limits.kMaxEntities)] Entity[] Entities  // this isn't actually part of the data chunk in a scenario!
	) : ILocoStruct
	{
		public bool Validate() => true;
	}

	[LocoStructSize(0x79D80)]
	public record GameStateC(
		[property: LocoStructOffset(0x0), LocoArrayLength((int)Limits.kMaxAnimations)] Animation[] Animations,
		[property: LocoStructOffset(0xC000), LocoArrayLength((int)Limits.kMaxWaves)] Wave[] Waves,
		[property: LocoStructOffset(0xC180), LocoArrayLength((int)Limits.kMaxUserStrings * 32)] uint8_t[] UserStrings,
		[property: LocoStructOffset(0x1C180), LocoArrayLength((int)(Limits.kMaxVehicles * Limits.kMaxRoutingsPerVehicle))] uint16_t[] Routings,
		[property: LocoStructOffset(0x3B580), LocoArrayLength((int)Limits.kMaxWaves)] uint8_t[] Orders
	) : ILocoStruct
	{
		public bool Validate() => true;
	}
}
