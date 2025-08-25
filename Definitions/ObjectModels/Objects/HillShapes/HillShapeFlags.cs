namespace Definitions.ObjectModels.Objects.HillShape;

[Flags]
public enum HillShapeFlags : uint16_t
{
	None = 0,
	IsHeightMap = 1 << 0,
}
