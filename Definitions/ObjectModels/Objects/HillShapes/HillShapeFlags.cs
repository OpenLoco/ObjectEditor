namespace Definitions.ObjectModels.Objects.HillShapes;

[Flags]
public enum HillShapeFlags : uint16_t
{
	None = 0,
	IsHeightMap = 1 << 0,
}
