using Definitions.ObjectModels.Objects.TownNames;

namespace Definitions.Database;

public class TblObjectTownNames : DbSubObject, IConvertibleToTable<TblObjectTownNames, TownNamesObject>
{
	public List<MorphemeCategory> MorphemeCategories { get; set; } = [];

	public static TblObjectTownNames FromObject(TblObject tbl, TownNamesObject obj)
		=> new()
		{
			Parent = tbl,
			MorphemeCategories = obj.MorphemeCategories,
		};
}
