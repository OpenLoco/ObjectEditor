using System.ComponentModel.DataAnnotations.Schema;

namespace OpenLoco.Definitions.Database
{
	public interface IDbDates
	{
		DateOnly? CreatedDate { get; }

		DateOnly? ModifiedDate { get; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		DateOnly UploadedDate { get; }
	}
}
