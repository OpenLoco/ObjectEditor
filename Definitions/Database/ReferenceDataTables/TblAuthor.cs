namespace OpenLoco.Definitions.Database
{
	public class TblAuthor : DbReferenceObject
	{
		public ICollection<TblObject> Objects { get; set; }
		public ICollection<TblObjectPack> ObjectPacks { get; set; }
		public ICollection<TblSC5File> SC5Files { get; set; }
		public ICollection<TblSC5FilePack> SC5FilePacks { get; set; }
	}
}
