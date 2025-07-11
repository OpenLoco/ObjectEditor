namespace Dat.Types.SCV5;

[Flags]
public enum ScenarioFlags : uint16_t
{
	None = (byte)0U,
	LandscapeGenerationDone = (byte)(1U << 0),
	HillsEdgeOfMap = (byte)(1U << 1),
}
