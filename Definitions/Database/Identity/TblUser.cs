using Microsoft.AspNetCore.Identity;
using OpenLoco.Definitions.Database;

namespace Definitions.Database.Identity
{
	public class TblUser : IdentityUser<DbKey>
	{
		public DbKey? AssociatedAuthorId { get; set; }
		public TblAuthor? AssociatedAuthor { get; set; }
	}
}
