namespace Definitions.ObjectModels.Objects.Dock;

[Flags]
public enum DockObjectFlags : uint16_t
{
	None = 0,
	HasShadows = 1 << 0,
}
