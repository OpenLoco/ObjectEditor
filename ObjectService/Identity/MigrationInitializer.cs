using Definitions.Database;
using Microsoft.EntityFrameworkCore;

namespace ObjectService.Identity;

/// <summary>
/// Bridges databases that were created before the project adopted EF migrations onto the migration
/// baseline.
/// <para>
/// Those databases were created with <c>EnsureCreated</c>, which does not create the
/// <c>__EFMigrationsHistory</c> table. Without a history row, <c>Migrate()</c> would try to create every
/// table from scratch and fail. When a non-empty database has no history table, we create one and record
/// the baseline migration as applied, so EF migrations take over from the current schema onwards.
/// </para>
/// <para>
/// Brand new databases are left untouched: <c>Migrate()</c> creates the full schema from the baseline.
/// </para>
/// </summary>
public static class MigrationInitializer
{
	public const string HistoryTableName = "__EFMigrationsHistory";

	/// <summary>
	/// Seeds the migration history for a pre-migrations database so the baseline is treated as applied.
	/// Safe to call on every startup; it is a no-op once the history table exists.
	/// </summary>
	public static async Task EnsureBaselineHistoryAsync(LocoDbContext db, ILogger logger)
	{
		if (await TableExistsAsync(db, HistoryTableName).ConfigureAwait(false))
		{
			return;
		}

		if (!await HasAnyUserTableAsync(db).ConfigureAwait(false))
		{
			// Brand new (or just-deleted) database: Migrate() will build the schema from the baseline.
			return;
		}

		var baselineMigrationId = db.Database.GetMigrations().FirstOrDefault();
		if (baselineMigrationId is null)
		{
			logger.LogWarning("Database already contains tables but no migrations were found in the assembly; skipping baseline seeding");
			return;
		}

		var productVersion = typeof(DbContext).Assembly.GetName().Version?.ToString() ?? "unknown";

#pragma warning disable EF1002 // table name is a compile-time constant, not user input
		await db.Database.ExecuteSqlRawAsync(
			$"""
			CREATE TABLE "{HistoryTableName}" (
			    "MigrationId" TEXT NOT NULL CONSTRAINT "PK_{HistoryTableName}" PRIMARY KEY,
			    "ProductVersion" TEXT NOT NULL
			)
			""").ConfigureAwait(false);
#pragma warning restore EF1002

		await db.Database
			.ExecuteSqlRawAsync(
				$"INSERT INTO \"{HistoryTableName}\" (\"MigrationId\", \"ProductVersion\") VALUES ({{0}}, {{1}})",
				baselineMigrationId,
				productVersion)
			.ConfigureAwait(false);

		logger.LogInformation(
			"Existing database has no migration history; recorded baseline migration {BaselineMigration} as applied",
			baselineMigrationId);
	}

	/// <summary>Returns the baseline migration id, i.e. the first migration in the assembly.</summary>
	public static string? GetBaselineMigrationId(LocoDbContext db)
		=> db.Database.GetMigrations().FirstOrDefault();

	static async Task<bool> TableExistsAsync(LocoDbContext db, string table)
	{
		var matches = await db.Database
			.SqlQueryRaw<string>("SELECT name AS \"Value\" FROM sqlite_master WHERE type = 'table' AND name = {0}", table)
			.ToListAsync()
			.ConfigureAwait(false);

		return matches.Count > 0;
	}

	static async Task<bool> HasAnyUserTableAsync(LocoDbContext db)
	{
		var matches = await db.Database
			.SqlQueryRaw<string>("SELECT name AS \"Value\" FROM sqlite_master WHERE type = 'table' AND name NOT LIKE 'sqlite_%'")
			.ToListAsync()
			.ConfigureAwait(false);

		return matches.Count > 0;
	}
}