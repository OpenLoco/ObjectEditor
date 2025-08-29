namespace Definitions.ObjectModels.Objects.Building;

[Flags]
public enum BuildingObjectFlags : uint8_t
{
	None = 0,
	LargeTile = 1 << 0, // 2x2 tile
	MiscBuilding = 1 << 1,
	Indestructible = 1 << 2,
	IsHeadquarters = 1 << 3,
}
