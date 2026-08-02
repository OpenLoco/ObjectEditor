using Definitions.Database.Base;

namespace Definitions.Database.DataTables;

public class TblSC5FilePack : DbCoreObject
{
	public ICollection<TblSC5File> SC5Files { get; set; } = [];
}
