using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public abstract class DbReferenceObject : DbIdObject, IDbName
	{
		public DbKey Id { get; set; }

		#region IDbName

		public required string Name { get; set; }

		#endregion
	}
}
