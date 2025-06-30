using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Id), IsUnique = true)]
	public abstract class DbSubObject : DbIdObject
	{
		public required TblObject Parent { get; set; }
	}
}
