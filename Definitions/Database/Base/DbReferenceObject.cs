using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public abstract class DbReferenceObject : DbIdObject, IDbName
	{
		public int Id { get; set; }
		public Guid GuidId { get; set; }

		#region IDbName

		public required string Name { get; set; }

		#endregion
	}
}
