using Definitions.Database.Base;
using Definitions.ObjectModels.Objects.TownNames;

namespace Definitions.Database.DataTables.Objects;

public class TblObjectTownNames : DbSubObject, IConvertibleToTable<TblObjectTownNames, TownNamesObject>
{
	//public ICollection<Category> Categories { get; set; }
	public static TblObjectTownNames FromObject(TblObject tbl, TownNamesObject obj)
		=> new()
		{
			Parent = tbl,
		};
}
