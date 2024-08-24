using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public class TblTag
	{
		public int TblTagId { get; set; }

		public string Name { get; set; }

		public ICollection<TblLocoObject> Objects { get; set; }
	}
}
