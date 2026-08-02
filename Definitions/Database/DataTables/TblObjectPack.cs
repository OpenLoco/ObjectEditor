using Definitions.Database.Base;

namespace Definitions.Database.DataTables;

public class TblObjectPack : DbCoreObject
{
	public ICollection<TblObject> Objects { get; set; } = [];
}
