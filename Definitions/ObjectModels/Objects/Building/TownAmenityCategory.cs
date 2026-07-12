namespace Definitions.ObjectModels.Objects.Building;

public enum TownAmenityCategory : uint8_t
{
	Religious = 0,
	Unk1 = 1, // No vanilla object uses this category
	Hotel = 2,
	Park = 3,
	Courthouse = 4,
	Landmark = 5, // e.g. a fountain
	Unk6 = 6,     // No vanilla object uses this category
	Unk7 = 7,     // No vanilla object uses this category
	None = 0xFF,  // Most buildings will have this category
};
