namespace OpenLoco.Definitions.Database
{
	public abstract class DbSubObject : DbIdObject
	{
		public required TblObject Parent { get; set; }
	}
}
