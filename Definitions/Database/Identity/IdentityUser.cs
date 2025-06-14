using OpenLoco.Definitions.Database;

namespace Definitions.Database.Identity
{
	public class IdentityUser : DbReferenceObjectGuid
	{
		public required string Email { get; set; }
		public required string PasswordHash { get; set; }
	}
}
