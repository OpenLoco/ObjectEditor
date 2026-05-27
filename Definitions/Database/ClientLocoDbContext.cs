using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

// Client-side projection of the Loco object schema. Used by the GUI's embedded
// ObjectService host so client databases never get ASP.NET Identity tables.
// No extra members today - reserved as the hook point for any future client-only schema.
public class ClientLocoDbContext : BaseLocoDbContext
{
	public ClientLocoDbContext()
	{ }

	public ClientLocoDbContext(DbContextOptions<ClientLocoDbContext> options) : base(options)
	{ }
}
