using System.ComponentModel.DataAnnotations.Schema;

namespace Definitions.Database
{
	public interface IDbDates
	{
		DateOnly? CreatedDate { get; }

		DateOnly? ModifiedDate { get; }

		[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
		DateOnly UploadedDate { get; }
	}
}
