using Microsoft.AspNetCore.Identity;

namespace Definitions.Database
{
	public class TblUserRole : IdentityRole<UniqueObjectId>, IHasId;
}
