namespace OpenLoco.Definitions.Database
{
	public class TblLocoObjectPack : DbCoreObject
	{
		public ICollection<TblLocoObject> Objects { get; set; }
	}
}
