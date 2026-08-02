using Definitions.Database.Base;
using Definitions.ObjectModels.Objects.Snow;

namespace Definitions.Database.DataTables.Objects;

public class TblObjectSnow : DbSubObject, IConvertibleToTable<TblObjectSnow, SnowObject>
{
	public static TblObjectSnow FromObject(TblObject tbl, SnowObject obj)
		=> new()
		{
			Parent = tbl,
		};
}
