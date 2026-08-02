using Definitions.Database.Base;
using Definitions.ObjectModels.Objects.Tunnel;

namespace Definitions.Database.DataTables.Objects;

public class TblObjectTunnel : DbSubObject, IConvertibleToTable<TblObjectTunnel, TunnelObject>
{
	public static TblObjectTunnel FromObject(TblObject tbl, TunnelObject obj)
		=> new()
		{
			Parent = tbl,
		};
}
