using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	public class TblLicence
	{
		public int TblLicenceId { get; set; }

		public string Name { get; set; }

		public string Text { get; set; }
	}
}
