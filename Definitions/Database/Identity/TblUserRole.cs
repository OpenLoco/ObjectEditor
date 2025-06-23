using Microsoft.AspNetCore.Identity;

namespace Definitions.Database.Identity
{
	public class TblUserRole : IdentityRole<DbKey>, IHasId
	{ }
}
