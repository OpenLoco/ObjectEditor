using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public abstract class DbCoreObject : DbIdObject, IDbName, IDbDescription, IDbMetadata, IDbDates
	{
		public int Id { get; set; }

		public Guid GuidId { get; set; }

		#region IDbName

		public required string Name { get; set; } // InternalName

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

		public DateTimeOffset? CreatedDate { get; set; }

		public DateTimeOffset? ModifiedDate { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed), NotNull]
		public DateTimeOffset UploadedDate { get; set; }

		#endregion
	}
}
