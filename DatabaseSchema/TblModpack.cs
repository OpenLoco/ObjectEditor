namespace OpenLoco.Db.Schema
{
	public class TblModpack
	{
		public int TblModpackId { get; set; }

		public string Name { get; set; }

		public ICollection<TblLocoObject> Objects { get; set; }
	}
}
