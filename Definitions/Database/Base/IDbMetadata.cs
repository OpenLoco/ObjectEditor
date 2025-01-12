using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoco.Definitions.Database
{
	public interface IDbMetadata
	{
		public ICollection<TblTag> Tags { get; set; }

		public TblLicence? Licence { get; set; }

		public ICollection<TblAuthor> Authors { get; set; }

		public DateTimeOffset? CreationDate { get; set; }

		public DateTimeOffset? LastEditDate { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		public DateTimeOffset UploadDate { get; set; }
	}
}
