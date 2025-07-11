using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Definitions.Database;

[Index(nameof(Name), IsUnique = true)]
public abstract class DbCoreObject : DbIdObject, IDbName, IDbDescription, IDbMetadata, IDbDates
{
	#region IDbName

	public required string Name { get; set; }

	#endregion

	#region IDbDescription

	public string? Description { get; set; }

	#endregion

	#region IDbMetadata

	public TblLicence? Licence { get; set; }

	public ICollection<TblAuthor> Authors { get; set; } = [];

	public ICollection<TblTag> Tags { get; set; } = [];

	#endregion

	#region IDbDates

	public DateOnly? CreatedDate { get; set; }

	public DateOnly? ModifiedDate { get; set; }

	[DatabaseGenerated(DatabaseGeneratedOption.Computed), NotNull]
	public DateOnly UploadedDate { get; set; }

	#endregion
}
