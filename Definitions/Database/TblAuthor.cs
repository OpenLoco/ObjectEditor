namespace OpenLoco.Definitions.Database
{
	public class TblAuthor
	{
		public int Id { get; set; }
		public required string Name { get; set; }

		public ICollection<TblLocoObject> Objects { get; set; }
	}
}
