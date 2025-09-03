namespace Definitions.ObjectModels.Objects.Airport;

[Flags]
public enum AirportMovementNodeFlags : uint16_t
{
	None = 0,
	Terminal = 1 << 0,
	TakeoffEnd = 1 << 1,
	Flag2 = 1 << 2,
	Taxiing = 1 << 3,
	InFlight = 1 << 4,
	HeliTakeoffBegin = 1 << 5,
	TakeoffBegin = 1 << 6,
	HeliTakeoffEnd = 1 << 7,
	Touchdown = 1 << 8,
}
