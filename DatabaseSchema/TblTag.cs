namespace OpenLoco.Db.Schema
{
	public class TblTag
	{
		public int TblTagId { get; set; }

		public string Name { get; set; }

		public ICollection<TblLocoObject> Objects { get; set; }
	}
}
