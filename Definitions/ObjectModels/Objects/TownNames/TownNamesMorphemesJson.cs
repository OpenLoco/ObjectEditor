namespace Definitions.ObjectModels.Objects.TownNames;

// used for json serialization of the morpheme categories for town names
public record TownNamesMorphemesJson(
	MorphemeCategory Category1,
	MorphemeCategory Category2,
	MorphemeCategory Category3,
	MorphemeCategory Category4,
	MorphemeCategory Category5,
	MorphemeCategory Category6);
