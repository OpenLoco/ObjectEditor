namespace Definitions.ObjectModels.Objects.Road;

[Flags]
public enum RoadObjectFlags : uint16_t
{
	None = 0,
	IsOneWay = 1 << 0,
	IsRailTransport = 1 << 1,
	unk_02 = 1 << 2,
	AnyRoadTypeCompatible = 1 << 3,
	NoWheelSlip = 1 << 4,
	unk_05 = 1 << 5,
	IsRoad = 1 << 6, // If not set this is tram track
	AllowUseByAllCompanies = 1 << 7, // If set, all companies can use this road. If unset, only the player company can use it.
	CanHaveStreetLights = 1 << 8,
}
