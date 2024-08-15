namespace Schema
{
	public class TblObjectTagLink
	{
		public int ObjectId { get; set; }
		public TblLocoObject Object { get; set; }
		public int TagId { get; set; }
		public TblTag Tag { get; set; }
	}
}
