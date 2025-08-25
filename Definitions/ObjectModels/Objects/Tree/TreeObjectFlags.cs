namespace Definitions.ObjectModels.Objects.Tree;

[Flags]
public enum TreeObjectFlags : uint16_t
{
	None = 0,
	HasSnowVariation = 1 << 0,
	unk_01 = 1 << 1,
	HighAltitude = 1 << 2,
	LowAltitude = 1 << 3,
	RequiresWater = 1 << 4,
	unk_05 = 1 << 5,
	DroughtResistant = 1 << 6,
	HasShadow = 1 << 7,
}
