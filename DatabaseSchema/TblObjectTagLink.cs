namespace OpenLoco.Db.Schema
{
	public class TblObjectTagLink
	{
		public int TblLocoObjectId { get; set; }
		public TblLocoObject Object { get; set; }

		public int TblTagId { get; set; }
		public TblTag Tag { get; set; }
	}
}
