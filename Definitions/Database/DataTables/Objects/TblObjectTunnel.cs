using Dat.Objects;

namespace Definitions.Database
{
	public class TblObjectTunnel : DbSubObject, IConvertibleToTable<TblObjectTunnel, TunnelObject>
	{
		public static TblObjectTunnel FromObject(TblObject tbl, TunnelObject obj)
			=> new()
			{
				Parent = tbl,
			};
	}
}
