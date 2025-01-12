using Microsoft.EntityFrameworkCore;
using OpenLoco.Dat.Data;

namespace OpenLoco.Definitions.Database
{
	// scenarios and landscapes, but no savegames
	[Index(nameof(Name), IsUnique = true)]
	public class TblSC5File : DbCoreObject
	{
		public ObjectSource ObjectSource { get; set; }

		public ICollection<TblSC5FilePack> SC5FilePacks { get; set; }
	}
}
