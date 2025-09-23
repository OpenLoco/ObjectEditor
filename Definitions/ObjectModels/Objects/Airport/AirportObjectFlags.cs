namespace Definitions.ObjectModels.Objects.Airport;

[Flags]
public enum AirportObjectFlags : uint16_t
{
	None = 0,
	HasShadows = 1 << 0,
	IsHelipad = 1 << 1,
	AcceptsLightPlanes = 1 << 2,
	AcceptsHeavyPlanes = 1 << 3,
	AcceptsHelicopter = 1 << 4,
}
