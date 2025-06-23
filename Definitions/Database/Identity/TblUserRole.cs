using Definitions;
using Microsoft.AspNetCore.Identity;

namespace OpenLoco.Definitions.Database
{
	public class TblUserRole : IdentityRole<DbKey>, IHasId;
}
