using System.Text.Json.Serialization;

namespace Definitions.ObjectModels.Objects.TownNames;

// used for json serialization of the morpheme categories for town names
public record TownNamesMorphemesJson(
	MorphemeCategory Category1,
	MorphemeCategory Category2,
	MorphemeCategory Category3,
	MorphemeCategory Category4,
	MorphemeCategory Category5,
	MorphemeCategory Category6);

public class MorphemeCategory
{
	public uint8_t Bias { get; set; }

	public List<StringTableEntry> TownNames { get; set; } = [];

	[JsonIgnore]
	public uint8_t DatCount { get; set; }

	[JsonIgnore]
	public uint16_t DatOffset { get; set; }
}
