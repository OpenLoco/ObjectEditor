namespace OpenLoco.Definitions.Database
{
	public class TblSC5FilePack : DbCoreObject
	{
		public ICollection<TblSC5File> SC5Files { get; set; } = [];
	}
}
