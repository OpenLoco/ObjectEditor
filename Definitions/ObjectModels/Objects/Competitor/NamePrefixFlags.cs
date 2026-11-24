namespace Definitions.ObjectModels.Objects.Competitor;

[Flags]
public enum NamePrefixFlags : uint32_t
{
	Ebony = 1 << 0,
	Silver = 1 << 1,
	Ivory = 1 << 2,
	Indigo = 1 << 3,
	Sapphire = 1 << 4,
	Emerald = 1 << 5,
	Golden = 1 << 6,
	Amber = 1 << 7,
	Bronze = 1 << 8,
	Bergundy = 1 << 9,
	Scarlet = 1 << 10,
	TownName = 1 << 11,
	Owner = 1 << 12,
}
