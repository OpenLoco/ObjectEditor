using Definitions.Database;
using Definitions.ObjectModels.Objects.TownNames;

namespace Definitions.DTO;

public class DtoObjectTownNames : IDtoSubObject
{
	public List<MorphemeCategory> MorphemeCategories { get; set; } = [];
	public UniqueObjectId Id { get; set; }
}
