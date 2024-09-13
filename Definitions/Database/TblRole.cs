using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public class TblRole
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public ICollection<TblUser> Users { get; set; }
	}
}
