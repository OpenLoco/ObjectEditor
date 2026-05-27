using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

// Server-side projection of the Loco object schema. Identity tables remain in the
// separate IdentityContext (server-only). No extra members today - reserved as the
// hook point for any future server-only schema additions (e.g. audit logs, jobs).
public class ServerLocoDbContext : BaseLocoDbContext
{
	public ServerLocoDbContext()
	{ }

	public ServerLocoDbContext(DbContextOptions<ServerLocoDbContext> options) : base(options)
	{ }
}
