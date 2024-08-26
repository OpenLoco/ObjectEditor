using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public class TblModpack
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public TblAuthor? Author { get; set; }

		public ICollection<TblLocoObject> Objects { get; set; }
	}
}
