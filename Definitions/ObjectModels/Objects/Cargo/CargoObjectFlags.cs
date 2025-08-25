namespace Definitions.ObjectModels.Objects.Cargo;

[Flags]
public enum CargoObjectFlags : uint8_t
{
	None = 0,
	unk0 = 1 << 0,
	Refit = 1 << 1,
	Delivering = 1 << 2,
}
