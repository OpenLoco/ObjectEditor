namespace OpenLoco.Definitions.Database
{
	public class TblObjectPack : DbCoreObject
	{
		public ICollection<TblObject> Objects { get; set; }
	}
}
