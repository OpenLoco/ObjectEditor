namespace Definitions.ObjectModels.Objects.Cargo;

public enum CargoCategory : uint16_t
{
	None = 0,
	Grain = 1,
	Coal = 2,
	IronOre = 3,
	Liquids = 4,
	Paper = 5,
	Steel = 6,
	Timber = 7,
	Goods = 8,
	Foods = 9,
	//<unused> = 10
	Livestock = 11,
	Passengers = 12,
	Mail = 13,
	// Note: Mods may (and do) use other numbers not covered by this list
	NULL = (uint16_t)0xFFFFU,
}
