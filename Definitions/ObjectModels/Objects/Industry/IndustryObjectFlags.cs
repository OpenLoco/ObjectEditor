namespace Definitions.ObjectModels.Objects.Industry;

[Flags]
public enum IndustryObjectFlags : uint32_t
{
	None = 0,
	BuiltInClusters = 1 << 0,
	BuiltOnHighGround = 1 << 1,
	BuiltOnLowGround = 1 << 2,
	BuiltOnSnow = 1 << 3,        // above summer snow line
	BuiltBelowSnowLine = 1 << 4, // below winter snow line
	BuiltOnFlatGround = 1 << 5,
	BuiltNearWater = 1 << 6,
	BuiltAwayFromWater = 1 << 7,
	BuiltOnWater = 1 << 8,
	BuiltNearTown = 1 << 9,
	BuiltAwayFromTown = 1 << 10,
	BuiltNearTrees = 1 << 11,
	BuiltRequiresOpenSpace = 1 << 12,
	Oilfield = 1 << 13,     // stations built nearby get named Oilfield
	Mines = 1 << 14,        // stations built nearby get named Mines
	NotRotatable = 1 << 15, // used on windmills
	CanBeFoundedByPlayer = 1 << 16,
	RequiresAllCargo = 1 << 17,
	CanIncreaseProduction = 1 << 18,
	CanDecreaseProduction = 1 << 19,
	RequiresElectricityPylons = 1 << 20,
	HasShadows = 1 << 21,
	KillsTrees = 1 << 22,
	FarmTilesGrowthStageDesynchronized = 1 << 23, // used by livestock farm, since it produces all the time. NOT used by regular farm, to keep the harvest roughly synchronized
	BuiltInDesert = 1 << 24,
	BuiltNearDesert = 1 << 25,
	FarmTilesDrawAboveSnow = 1 << 26,    // used by skislopes, otherwise farm tiles draw below snow
	FarmTilesPartialCoverage = 1 << 27,  // used by skislopes to randomly skip 7/8 of tiles when creating 5x5 fields
	FarmProductionIgnoresSnow = 1 << 28, // used by forest
}
