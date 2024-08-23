using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Schema.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public class TblLicence
	{
		public int TblLicenceId { get; set; }

		public string Name { get; set; }

		public string Text { get; set; }
	}
}