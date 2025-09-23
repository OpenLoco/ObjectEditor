namespace Definitions.ObjectModels.Objects.Steam;

[Flags]
public enum SteamObjectFlags : uint16_t
{
	None = 0,
	ApplyWind = 1 << 0,
	DisperseOnCollision = 1 << 1,
	HasTunnelSounds = 1 << 2,
	unk_03 = 1 << 3,
}
