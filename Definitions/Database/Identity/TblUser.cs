using Definitions;
using Microsoft.AspNetCore.Identity;

namespace OpenLoco.Definitions.Database
{
	public class TblUser : IdentityUser<DbKey>, IHasId
	{
		public DbKey? AssociatedAuthorId { get; set; }
		public TblAuthor? AssociatedAuthor { get; set; }
	}
}
