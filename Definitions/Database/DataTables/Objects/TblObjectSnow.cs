using Dat.Objects;

namespace Definitions.Database;

public class TblObjectSnow : DbSubObject, IConvertibleToTable<TblObjectSnow, SnowObject>
{
	public static TblObjectSnow FromObject(TblObject tbl, SnowObject obj)
		=> new()
		{
			Parent = tbl,
		};
}
