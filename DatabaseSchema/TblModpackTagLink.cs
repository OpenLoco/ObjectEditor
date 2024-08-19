namespace OpenLoco.Db.Schema
{
	public class TblModpackTagLink
	{
		public int TblLocoObjectId { get; set; }
		public TblLocoObject Object { get; set; }

		public int TblModpackId { get; set; }
		public TblModpack Modpack { get; set; }
	}
}
