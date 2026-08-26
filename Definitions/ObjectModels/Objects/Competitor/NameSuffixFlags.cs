namespace Definitions.ObjectModels.Objects.Competitor;

[Flags]
public enum NameSuffixFlags : uint32_t
{
	Transport = 1 << 0,
	Express = 1 << 1,
	Lines = 1 << 2,
	Tracks = 1 << 3,
	Coaches = 1 << 4,
	Air = 1 << 5,
	Rail = 1 << 6,
	Carts = 1 << 7,
	Trains = 1 << 8,
	Haulage = 1 << 9,
	Shipping = 1 << 10,
	Freight = 1 << 11,
	Trucks = 1 << 12,
}
