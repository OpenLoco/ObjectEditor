using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Db.Schema
{
	[Index(nameof(Name), IsUnique = true)]
	public class TblModpack
	{
		public int TblModpackId { get; set; }

		public string Name { get; set; }

		public ICollection<TblLocoObject> Objects { get; set; }
	}
}
