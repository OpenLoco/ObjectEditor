using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

[Index(nameof(Name), IsUnique = true)]
public abstract class DbReferenceObject : DbIdObject, IDbName
{
	#region IDbName

	public required string Name { get; set; }

	#endregion
}
