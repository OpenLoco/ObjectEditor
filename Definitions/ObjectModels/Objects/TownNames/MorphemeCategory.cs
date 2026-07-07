namespace Definitions.ObjectModels.Objects.TownNames;

public class MorphemeCategory
{
	public uint8_t DatCount { get; set; }

	public uint8_t Bias { get; set; }

	public uint16_t DatOffset { get; set; }

	public List<StringTableEntry> TownNames { get; set; } = [];
}
