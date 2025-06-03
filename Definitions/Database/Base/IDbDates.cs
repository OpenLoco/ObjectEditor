using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoco.Definitions.Database
{
	public interface IDbDates
	{
		DateTimeOffset? CreatedDate { get; set; }

		DateTimeOffset? ModifiedDate { get; set; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		DateTimeOffset UploadedDate { get; set; }
	}
}
