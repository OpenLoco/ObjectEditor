// DAT/S5 binary parsing — nullable analysis cannot reason about offset-based field population.
#pragma warning disable CS8618, CS8602, CS8604, CS8601, CS8625, CS8629

using Dat.FileParsing;
using Definitions.ObjectModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Dat.Types.SCV5;

[TypeConverter(typeof(ExpandableObjectConverter))]
[LocoStructSize(0x8DC8)]
public record CompanyType2(
	[property: LocoStructOffset(0x00)] uint16_t Name,
	[property: LocoStructOffset(0x02)] uint16_t OwnerName,
	[property: LocoStructOffset(0x04)] CompanyFlags ChallengeFlags,
	[property: LocoStructOffset(0x08), LocoArrayLength(6)] uint8_t[] Cash,               // currency48_t
	[property: LocoStructOffset(0x0E)] uint32_t CurrentLoan,
	[property: LocoStructOffset(0x12)] uint32_t UpdateCounter,
	[property: LocoStructOffset(0x16)] int16_t PerformanceIndex,
	[property: LocoStructOffset(0x18)] uint8_t CompetitorId,
	[property: LocoStructOffset(0x19)] uint8_t OwnerEmotion,
	[property: LocoStructOffset(0x1A), LocoArrayLength(2)] uint8_t[] MainColours,
	[property: LocoStructOffset(0x1C), LocoArrayLength(20)] uint8_t[] VehicleColours,     // [10][2]
	[property: LocoStructOffset(0x30)] uint32_t CustomVehicleColoursSet,
	[property: LocoStructOffset(0x34), LocoArrayLength(7)] uint32_t[] UnlockedVehicles,
	[property: LocoStructOffset(0x50)] uint16_t AvailableVehicles,
	[property: LocoStructOffset(0x52)] uint32_t AiPlaystyleFlags,
	[property: LocoStructOffset(0x56)] uint8_t AiPlaystyleTownId,
	[property: LocoStructOffset(0x57)] uint8_t NumExpenditureYears,
	[property: LocoStructOffset(0x58), LocoArrayLength(272)] uint32_t[] Expenditures,     // [16][17] currency32_t
	[property: LocoStructOffset(0x0498)] uint32_t StartedDate,
	[property: LocoStructOffset(0x049C)] uint32_t var_49C,
	[property: LocoStructOffset(0x04A0)] uint32_t var_4A0,
	[property: LocoStructOffset(0x04A4)] uint8_t var_4A4,
	[property: LocoStructOffset(0x04A5)] uint8_t var_4A5,
	[property: LocoStructOffset(0x04A6)] uint8_t var_4A6,
	[property: LocoStructOffset(0x04A7)] uint8_t var_4A7,
	[property: LocoStructOffset(0x04A8), LocoArrayLength(60 * 0x8C)] uint8_t[] AiThoughts,    // AiThought[60], sizeof(AiThought)=0x8C
	[property: LocoStructOffset(0x2578)] uint8_t ActiveThoughtId,
	[property: LocoStructOffset(0x2579)] uint8_t HeadquartersZ,                               // World::SmallZ
	[property: LocoStructOffset(0x257A)] int16_t HeadquartersX,                               // coord_t, -1 on no headquarter placed
	[property: LocoStructOffset(0x257C)] int16_t HeadquartersY,                               // coord_t
	[property: LocoStructOffset(0x257E)] uint32_t ActiveThoughtRevenueEstimate,               // currency32_t
	[property: LocoStructOffset(0x2582)] uint32_t var_2582,
	[property: LocoStructOffset(0x2586), LocoArrayLength(0x2596 - 0x2586)] uint8_t[] pad_2586,
	[property: LocoStructOffset(0x2596)] uint32_t var_2596,
	[property: LocoStructOffset(0x259A)] uint8_t var_259A,
	[property: LocoStructOffset(0x259B)] uint8_t var_259B,
	[property: LocoStructOffset(0x259C)] uint8_t var_259C,
	[property: LocoStructOffset(0x259D)] uint8_t pad_259D,
	[property: LocoStructOffset(0x259E)] uint32_t AiPlaceVehicleIndex,
	[property: LocoStructOffset(0x25A2), LocoArrayLength(0x25BE - 0x25A2)] uint8_t[] pad_25A2,
	[property: LocoStructOffset(0x25BE)] uint8_t var_25BE,
	[property: LocoStructOffset(0x25BF)] uint8_t CurrentRating,
	[property: LocoStructOffset(0x25C0), LocoArrayLength(0x1000 * 6)] uint8_t[] var_25C0,     // HashTableEntry[0x1000]
	[property: LocoStructOffset(0x85C0)] uint16_t var_25C0_length,
	[property: LocoStructOffset(0x85C2)] uint8_t var_85C2,
	[property: LocoStructOffset(0x85C3)] uint8_t var_85C3,
	[property: LocoStructOffset(0x85C4)] DatPos2 var_85C4,                                    // World::Pos2
	[property: LocoStructOffset(0x85C8)] uint8_t var_85C8,                                    // World::SmallZ
	[property: LocoStructOffset(0x85C9)] DatPos2 var_85C9,
	[property: LocoStructOffset(0x85CD)] uint8_t var_85CD,                                    // World::SmallZ
	[property: LocoStructOffset(0x85CE)] uint8_t var_85CE,
	[property: LocoStructOffset(0x85CF)] uint8_t var_85CF,
	[property: LocoStructOffset(0x85D0)] DatPos2 var_85D0,
	[property: LocoStructOffset(0x85D4)] uint8_t var_85D4,                                    // World::SmallZ
	[property: LocoStructOffset(0x85D5)] uint16_t var_85D5,
	[property: LocoStructOffset(0x85D7)] DatPos2 var_85D7,
	[property: LocoStructOffset(0x85DB)] uint8_t var_85DB,                                    // World::SmallZ
	[property: LocoStructOffset(0x85DC)] uint16_t var_85DC,
	[property: LocoStructOffset(0x85DE)] uint32_t var_85DE,
	[property: LocoStructOffset(0x85E2)] uint32_t var_85E2,
	[property: LocoStructOffset(0x85E6)] uint16_t var_85E6,
	[property: LocoStructOffset(0x85E8)] uint16_t var_85E8,
	[property: LocoStructOffset(0x85EA)] uint32_t var_85EA,
	[property: LocoStructOffset(0x85EE)] uint8_t var_85EE,
	[property: LocoStructOffset(0x85EF)] uint8_t var_85EF,
	[property: LocoStructOffset(0x85F0)] uint16_t var_85F0,
	[property: LocoStructOffset(0x85F2)] uint32_t var_85F2,                                   // currency32_t
	[property: LocoStructOffset(0x85F6)] uint16_t var_85F6,
	[property: LocoStructOffset(0x85F8)] uint32_t CargoUnitsTotalDelivered,
	[property: LocoStructOffset(0x85FC), LocoArrayLength(120)] uint32_t[] CargoUnitsDeliveredHistory,
	[property: LocoStructOffset(0x87DC), LocoArrayLength(120)] int16_t[] PerformanceIndexHistory,
	[property: LocoStructOffset(0x88CC)] uint16_t HistorySize,
	[property: LocoStructOffset(0x88CE), LocoArrayLength(120 * 6)] uint8_t[] CompanyValueHistory,   // currency48_t[120]
	[property: LocoStructOffset(0x8B9E), LocoArrayLength(6)] uint8_t[] VehicleProfit,               // currency48_t
	[property: LocoStructOffset(0x8BA4), LocoArrayLength(6)] uint16_t[] TransportTypeCount,
	[property: LocoStructOffset(0x8BB0), LocoArrayLength(9)] uint8_t[] ActiveEmotions,
	[property: LocoStructOffset(0x8BB9)] uint8_t ObservationStatus,
	[property: LocoStructOffset(0x8BBA)] uint16_t ObservationTownId,
	[property: LocoStructOffset(0x8BBC)] uint16_t ObservationEntity,
	[property: LocoStructOffset(0x8BBE)] int16_t ObservationX,
	[property: LocoStructOffset(0x8BC0)] int16_t ObservationY,
	[property: LocoStructOffset(0x8BC2)] uint16_t ObservationObject,
	[property: LocoStructOffset(0x8BC4)] uint16_t ObservationTimeout,
	[property: LocoStructOffset(0x8BC6), LocoArrayLength(2)] uint16_t[] OwnerStatus,
	[property: LocoStructOffset(0x8BCA), LocoArrayLength(0x8BCE - 0x8BCA)] uint8_t[] pad_8BCA,
	[property: LocoStructOffset(0x8BCE), LocoArrayLength(32)] uint32_t[] CargoDelivered,
	[property: LocoStructOffset(0x8C4E)] uint8_t ChallengeProgress,
	[property: LocoStructOffset(0x8C4F)] uint8_t NumMonthsInTheRed,
	[property: LocoStructOffset(0x8C50)] uint32_t CargoUnitsTotalDistance,
	[property: LocoStructOffset(0x8C54)] uint16_t JailStatus,
	[property: LocoStructOffset(0x8C56), LocoArrayLength(0x8DC8 - 0x8C56)] uint8_t[] pad_8C56
	)
	: ILocoStruct
{
	public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		=> [];
}
