namespace Definitions.ObjectModels.Objects.Bridge;

[Flags]
public enum BridgeObjectFlags : uint8_t
{
	None = 0,
	HasRoof = 1 << 0,
}
