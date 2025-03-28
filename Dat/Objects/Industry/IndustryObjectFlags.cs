namespace OpenLoco.Dat.Objects
{
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
		Oilfield = 1 << 13,
		Mines = 1 << 14,
		NotRotatable = 1 << 15,
		CanBeFoundedByPlayer = 1 << 16,
		RequiresAllCargo = 1 << 17,
		CanIncreaseProduction = 1 << 18,
		CanDecreaseProduction = 1 << 19,
		RequiresElectricityPylons = 1 << 20,
		HasShadows = 1 << 21,
		unk_22 = 1 << 22,
		unk_23 = 1 << 23,
		BuiltInDesert = 1 << 24,
		BuiltNearDesert = 1 << 25,
		unk_26 = 1 << 26,
		unk_27 = 1 << 27,
		unk_28 = 1 << 28,
	}
}
