using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Objects.TownNames;

public class MorphemeCategory
{
	public uint8_t Bias { get; set; }

	public List<StringTableEntry> TownNames { get; set; } = [];

	[JsonIgnore]
	public uint8_t DatCount { get; set; }

	[JsonIgnore]
	public uint16_t DatOffset { get; set; }
}
