using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public class TblLicence : DbReferenceObject
	{
		public string Text { get; set; }
	}
}
