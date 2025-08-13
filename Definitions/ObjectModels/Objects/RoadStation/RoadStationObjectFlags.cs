namespace Definitions.ObjectModels.Objects.RoadStation;

[Flags]
public enum RoadStationObjectFlags : uint8_t
{
	None = 0,
	Recolourable = 1 << 0,
	Passenger = 1 << 1,
	Freight = 1 << 2,
	RoadEnd = 1 << 3,
}
