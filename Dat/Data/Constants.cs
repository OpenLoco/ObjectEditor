namespace Dat.Data;

public static class Constants
{
	public const int LocoDatFileFlag = 0x11;
	public const int G1ObjectTabsOffset = 3505; // see ImageIds.h in openloco
}

public static class Limits
{
	public const size_t kMaxMessages = 199;
	public const size_t kMaxCompanies = 15;
	public const size_t kMinTowns = 1;
	public const size_t kMaxTowns = 80;
	public const size_t kMaxIndustries = 128;
	public const size_t kMaxStations = 1024;
	public const size_t kMaxEntities = 20000;
	public const size_t kMaxAnimations = 8192;
	public const size_t kMaxWaves = 64;
	public const size_t kMaxUserStrings = 2048;
	public const size_t kMaxVehicles = 1000;
	public const size_t kMaxRoutingsPerVehicle = 64;
	// The number of orders appears to be the number of routings minus a null byte (OrderEnd)
	public const size_t kMaxOrdersPerVehicle = kMaxRoutingsPerVehicle - 1;
	public const size_t kMaxOrders = 256000;
	public const size_t kNumEntityLists = 7;
	// There is a separate pool of 200 entities dedicated for money
	public const size_t kMaxMoneyEntities = 200;
	// This is the main pool for everything that isn't money
	public const size_t kMaxNormalEntities = kMaxEntities - kMaxMoneyEntities;
	// Money is not counted in this limit
	public const size_t kMaxMiscEntities = 4000;
	public const int kMapColumnsVanilla = 384;
	public const int kMapRowsVanilla = 384;

	public const int kMaxObjectTypes = 34;
	public const int kMaxSawyerEncodings = 4;
}
