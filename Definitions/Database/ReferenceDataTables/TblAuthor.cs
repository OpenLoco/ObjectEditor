namespace OpenLoco.Definitions.Database
{
	public class TblAuthor
	{
		public int Id { get; set; }
		public required string Name { get; set; }
		public ICollection<TblLocoObject> Objects { get; set; }
		public ICollection<TblLocoObjectPack> ObjectPacks { get; set; }
		public ICollection<TblSC5File> SC5Files { get; set; }
		public ICollection<TblSC5FilePack> SC5FilePacks { get; set; }
	}
}
