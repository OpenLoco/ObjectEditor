using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Definitions.Database;

/// <summary>
/// Design-time factory that lets <c>dotnet ef migrations</c> build the model without starting the web
/// app (which would require the configured GameData folders and secrets). Migrations only need the
/// model, so a throwaway SQLite connection string is enough.
/// </summary>
public sealed class LocoDbContextDesignTimeFactory : IDesignTimeDbContextFactory<LocoDbContext>
{
	public LocoDbContext CreateDbContext(string[] args)
	{
		var options = new DbContextOptionsBuilder<LocoDbContext>()
			.UseSqlite("Data Source=design-time.db")
			.Options;

		return new LocoDbContext(options);
	}
}
