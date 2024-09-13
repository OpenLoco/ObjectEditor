using Microsoft.EntityFrameworkCore;

namespace OpenLoco.Definitions.Database
{
	[Index(nameof(Name), IsUnique = true)]
	[Index(nameof(DisplayName), IsUnique = true)]
	[Index(nameof(PasswordSalt), IsUnique = true)]
	public class TblUser
	{
		public int Id { get; set; }
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public TblAuthor? Author { get; set; }

		public ICollection<TblRole> Roles { get; set; }

		public string PasswordHashed { get; set; }
		public string PasswordSalt { get; set; }

		public DateTimeOffset CreatedDate { get; set; }
	}
}
