using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public class TblTag
	{
		public int Id { get; set; }

		public string Name { get; set; }
	}
}
