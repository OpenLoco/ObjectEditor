using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Definitions.Database;

// Holds the ASP.NET Core Identity schema only. Shares the same SQLite file as LocoDbContext
// but tracks migrations in a separate __EFMigrationsHistory_Identity table.
//
// This context is server-only. Clients running purely against LocoDbContext will not have
// Identity tables in their local DB and will not register this context with DI.
public class IdentityContext : IdentityDbContext<TblUser, TblUserRole, UniqueObjectId>
{
	public const string MigrationsHistoryTableName = "__EFMigrationsHistory_Identity";

	public IdentityContext()
	{ }

	public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
	{ }

	protected override void OnConfiguring(DbContextOptionsBuilder builder)
	{
		if (!builder.IsConfigured)
		{
			// Default to the same SQLite file used by LocoDbContext for the design-time/CLI case
			_ = builder.UseSqlite(
				$"Data Source={LocoDbContext.DefaultDb}",
				sql => sql.MigrationsHistoryTable(MigrationsHistoryTableName));
		}
	}
}
