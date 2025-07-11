using Microsoft.EntityFrameworkCore;

namespace Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public class TblLicence : DbReferenceObject
	{
		public required string Text { get; set; }
	}
}
